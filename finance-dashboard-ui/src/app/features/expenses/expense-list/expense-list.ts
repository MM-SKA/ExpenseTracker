import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { ExpenseService } from '../../../core/services/expense';
import { CategoryService } from '../../../core/services/category.service';
import { StorageService } from '../../../core/services/storage.service';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-expense-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslatePipe],
  templateUrl: './expense-list.html',
  styleUrl: './expense-list.css',
})
export class ExpenseList implements OnInit {

  expenses: any[] = [];
  categories: any[] = [];
  selectedCategory = '';
  totalRecords = 0;
  totalPagesCount = 1;
  analytics: any = null;

  filterText = '';
  startDate = '';
  endDate = '';

  sortOrder: 'recent' | 'oldest' = 'recent';
  currentPage: number = 1;
  pageSize: number = 10;

  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly expenseService = inject(ExpenseService);
  private readonly cdr = inject(ChangeDetectorRef);
  private readonly categoryService = inject(CategoryService);
  private readonly storageService = inject(StorageService);
  private readonly translate = inject(TranslateService);

  ngOnInit(): void {

    this.route.queryParams.subscribe(params => {
      this.selectedCategory = params['category'] ?? '';
      this.filterText = params['search'] ?? '';
      this.startDate = params['startDate'] ?? '';
      this.endDate = params['endDate'] ?? '';
      this.sortOrder = params['sort'] === 'oldest' ? 'oldest' : 'recent';
      this.currentPage = Number(params['page']) || 1;
      this.pageSize = Number(params['pageSize']) || 10;
      this.loadExpenses();
    });
    this.categoryService
      .getCategories()
      .subscribe({
        next: categories => {
          this.categories = categories;
        }
      });
  }

  loadExpenses(): void {

    this.expenseService
      .filterPagedExpenses({
        pageNumber: this.currentPage,
        pageSize: this.pageSize,
        categoryId: this.selectedCategory || null,
        startDate: this.startDate || null,
        endDate: this.endDate || null,
        notes: this.filterText || null,
        location: null,
        sortOrder: this.sortOrder,
        includeAnalytics: true
      })
      .subscribe({
        next: (response: any) => {

          const data =
            response?.data ?? response;

          this.expenses =
            data.items ?? [];

          this.currentPage =
            data.pageNumber ?? 1;

          this.pageSize =
            data.pageSize ?? this.pageSize;

          this.totalRecords =
            data.totalRecords ?? 0;

          this.totalPagesCount =
            data.totalPages ?? 1;

          this.analytics =
            data.analytics ?? null;

          this.cdr.detectChanges();
        },
        error: error => {
          console.error(error);
        }
      });
  }

  get totalPages(): number {
    return this.totalPagesCount;
  }

  get startIndex(): number {

    if (this.totalRecords === 0) {
      return 0;
    }
    return ((this.currentPage - 1) * this.pageSize) + 1;
  }

  get endIndex(): number {
    return Math.min(this.currentPage * this.pageSize, this.totalRecords);
  }

  get pages(): number[] {
    const total = this.totalPages;
    const maxVisible = 5;
    let start = Math.max(1, this.currentPage - Math.floor(maxVisible / 2));
    let end = start + maxVisible - 1;

    if (end > total) {
      end = total;
      start = Math.max(1, end - maxVisible + 1);
    }

    const pagesArray: number[] = [];
    for (let i = start; i <= end; i++) {
      pagesArray.push(i);
    }
    return pagesArray;
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.updateQueryParams();
    }
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.updateQueryParams();
    }
  }

  nextPage(): void {
    if (this.currentPage < this.totalPagesCount) {
      this.currentPage++;
      this.updateQueryParams();
    }
  }

  getTotalAmount(): number {
    return this.analytics?.summary?.totalSpent
      ?? this.analytics?.Summary?.TotalSpent
      ?? 0;
  }

  getAverageAmount(): number {
    return this.analytics?.summary?.averageAmount
      ?? this.analytics?.Summary?.AverageAmount
      ?? 0;
  }

  getHighestAmount(): number {
    return this.analytics?.summary?.maxAmount
      ?? this.analytics?.Summary?.MaxAmount
      ?? 0;
  }

  getAmountBadgeClass(amount: number): string {
    if (amount > 50) {
      return 'bg-danger';
    }
    if (amount > 20) {
      return 'bg-warning';
    }
    return 'bg-success';
  }

  onFilterChange(): void {
    this.currentPage = 1;
    this.updateQueryParams();
  }

  onSortChange(): void {
    this.currentPage = 1;
    this.updateQueryParams();
  }

  onPageSizeChange(): void {
    this.currentPage = 1;
    this.updateQueryParams();
  }

  clearFilter(): void {
    this.filterText = '';
    this.startDate = '';
    this.endDate = '';
    this.selectedCategory = '';
    this.sortOrder = 'recent';
    this.currentPage = 1;
    this.pageSize = 10;
    this.router.navigate([], { relativeTo: this.route, queryParams: {} });
  }

  delete(id: string): void {
    const confirmed =
      confirm(
        'Are you sure you want to delete this expense?'
      );

    if (!confirmed) {
      return;
    }
    this.expenseService
      .deleteExpense(id)
      .subscribe({
        next: () => {
          if (
            this.expenses.length === 1 &&
            this.currentPage > 1
          ) {
            this.currentPage--;
          }

          this.loadExpenses();
        },
        error: (error) => {
          console.error(error);
          alert(
            'Unable to delete expense.'
          );
        }
      });
  }

  public editExpense(id: string): void {
    this.router.navigate(['/expenses/edit', id]);
  }

  private updateQueryParams(): void {
    void this.router.navigate(
      [], {
      relativeTo: this.route,
      queryParams: {
        category: this.selectedCategory || null,
        search: this.filterText || null,
        startDate: this.startDate || null,
        endDate: this.endDate || null,
        sort: this.sortOrder,
        page: this.currentPage,
        pageSize: this.pageSize
      }
    }
    );
  }

  downloadExcel(): void {

    this.expenseService
      .exportExpenses({
        pageNumber: 1,
        pageSize: 100,
        categoryId: this.selectedCategory || null,
        startDate: this.startDate || null,
        endDate: this.endDate || null,
        minAmount: null,
        maxAmount: null,
        notes: this.filterText || null,
        location: null,
        sortOrder: this.sortOrder,
        includeAnalytics: false
      })
      .subscribe({
        next: blob => {
          const url =
            window.URL.createObjectURL(blob);
          const anchor =
            document.createElement('a');
          anchor.href = url;
          anchor.download =
            `expenses-${new Date()
              .toISOString()
              .slice(0, 10)}.xlsx`;
          anchor.click();
          window.URL.revokeObjectURL(url);
        },
        error: error => {
          console.error(error);
        }
      });
  }

}
