import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ExpenseService } from '../../../core/services/expense';
import { CategoryService } from '../../../core/services/category.service';

@Component({
  selector: 'app-expense-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
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

  private readonly router = inject(Router);
  private readonly expenseService = inject(ExpenseService);
  private readonly cdr = inject(ChangeDetectorRef);
  private readonly categoryService = inject(CategoryService);

  ngOnInit(): void {
    const token = typeof localStorage !== 'undefined' ? localStorage.getItem('token') : null;
    if (token) {
      this.expenseService.getExpenses().subscribe({
        next: (response: any) => {
          const data = response?.data ?? response;
          this.expenses = this.sortExpenses(Array.isArray(data) ? data : []);
          this.cdr.detectChanges();
        },
        error: () => this.load()
      });
      this.categoryService
        .getCategories()
        .subscribe({
          next: (categories) => {
            this.categories =
              categories;
          }
        });
    } else {
      this.load();
    }
  }

  load(): void {
    if (typeof localStorage !== 'undefined') {
      try {
        this.expenses = this.sortExpenses(JSON.parse(localStorage.getItem('expenses') || '[]'));
      } catch {
        this.expenses = [];
      }
    } else {
      this.expenses = [];
    }
  }

  private sortExpenses(expenses: any[]): any[] {
    return [...expenses].sort((a, b) => {
      const aTime = Date.parse(a?.date ?? '');
      const bTime = Date.parse(b?.date ?? '');
      if (!Number.isNaN(aTime) && !Number.isNaN(bTime)) {
        return bTime - aTime;
      }
      return (b?.id ?? 0) - (a?.id ?? 0);
    });
  }

  filtered(): any[] {
    const filter = this.filterText?.trim().toLowerCase();
    return this.expenses.filter(e => {
      if (filter && !(e.description ?? '').toLowerCase().includes(filter)) { return false; }
      if (this.startDate && e.date < this.startDate) { return false; }
      if (this.endDate && e.date > this.endDate) { return false; }
      if (this.selectedCategory && e.categoryName !== this.selectedCategory) { return false; }
      return true;
    });
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
    // Change detection is triggered by ngModel changes.
  }

  clearFilter(): void {
    this.filterText = '';
    this.startDate = '';
    this.endDate = '';
    this.selectedCategory = '';
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
                this.expenses =
                  this.sortExpenses(
                    Array.isArray(data)
                      ? data
                      : []
                  );
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
