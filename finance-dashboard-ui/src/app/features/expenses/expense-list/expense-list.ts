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
          this.categories = categories;
        }
      });
  }

  loadExpenses(): void {

    this.expenseService
      .getExpenses()
      .subscribe({
        next: (response: any) => {

          const data =
            response?.data ?? response;

          this.expenses =
            Array.isArray(data)
              ? data
              : [];
          this.cdr.detectChanges();
        },
        error: error => {
          console.error(error);
        }
      });
  }

  load(): void {
    this.expenses = this.storageService.getEncrypted<any[]>('expenses') || [];
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

  get paginatedExpenses(): any[] {
    const filteredList = this.filtered();
    const start = (this.currentPage - 1) * this.pageSize;
    return filteredList.slice(start, start + this.pageSize);
  }

  get totalPages(): number {
    const count = this.filtered().length;
    return count === 0 ? 1 : Math.ceil(count / this.pageSize);
  }

  get startIndex(): number {
    const total = this.filtered().length;
    if (total === 0) return 0;
    return (this.currentPage - 1) * this.pageSize + 1;
  }

  get endIndex(): number {
    const total = this.filtered().length;
    return Math.min(this.currentPage * this.pageSize, total);
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
    }
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
    }
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
    }
  }

  getTotalAmount(): number {
    return this.filtered().reduce((sum, e) => sum + (e.amount ?? 0), 0);
  }

  getAverageAmount(): number {
    const filtered = this.filtered();
    if (filtered.length === 0) {
      return 0;
    }
    return this.getTotalAmount() / filtered.length;
  }

  getHighestAmount(): number {
    const list = this.filtered();
    if (list.length === 0) return 0;
    return Math.max(...list.map(e => Number(e.amount) || 0));
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
  }

  onSortChange(): void {
    this.currentPage = 1;
  }

  onPageSizeChange(): void {
    this.currentPage = 1;
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
