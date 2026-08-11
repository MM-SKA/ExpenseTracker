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
      if (params['category']) {
        this.selectedCategory = params['category'];
        this.currentPage = 1;
        this.cdr.detectChanges();
      }
    });

    this.loadExpenses();
    this.categoryService
      .getCategories()
      .subscribe({
        next: categories => {
          console.log(categories);
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

  private sortExpenses(expenses: any[]): any[] {
    return [...expenses].sort((a, b) => {
      const aTime = Date.parse(a?.date ?? '');
      const bTime = Date.parse(b?.date ?? '');
      let comparison = 0;
      if (!Number.isNaN(aTime) && !Number.isNaN(bTime)) {
        comparison = aTime - bTime;
      } else {
        const aId = String(a?.id ?? '');
        const bId = String(b?.id ?? '');
        comparison = aId.localeCompare(bId);
      }
      return this.sortOrder === 'recent' ? -comparison : comparison;
    });
  }

  filtered(): any[] {
    const filter = this.filterText?.trim().toLowerCase();
    const catFilter = this.selectedCategory?.trim().toLowerCase();

    const result = this.expenses.filter(e => {
      if (filter && !(e.description ?? '').toLowerCase().includes(filter)) { return false; }
      if (this.startDate && e.date < this.startDate) { return false; }
      if (this.endDate && e.date > this.endDate) { return false; }
      if (catFilter) {
        const eCatName = (e.categoryName ?? '').trim().toLowerCase();
        const eCatId = (e.categoryId ?? '').trim().toLowerCase();
        if (eCatName !== catFilter && eCatId !== catFilter) {
          return false;
        }
      }
      return true;
    });
    return this.sortExpenses(result);
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
      this.loadExpenses();
    }
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadExpenses();
    }
  }

  nextPage(): void {
    if (this.currentPage < this.totalPagesCount) {
      this.currentPage++;
      this.loadExpenses();
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
    this.loadExpenses();
  }

  onSortChange(): void {
    this.currentPage = 1;
    this.loadExpenses();
  }

  onPageSizeChange(): void {
    this.currentPage = 1;
    this.loadExpenses();
  }

  clearFilter(): void {
    this.filterText = '';
    this.startDate = '';
    this.endDate = '';
    this.selectedCategory = '';
    this.sortOrder = 'recent';
    this.currentPage = 1;
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
          this.expenseService
            .getExpenses()
            .subscribe({
              next: (response: any) => {
                const data =
                  response?.data ?? response;
                this.expenses = Array.isArray(data) ? data : [];
                this.storageService.setEncrypted('expenses', this.expenses);
                if (this.currentPage > this.totalPages) {
                  this.currentPage = Math.max(1, this.totalPages);
                }
                this.cdr.detectChanges();
              }
            });
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

}
