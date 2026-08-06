import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ExpenseService } from '../../../core/services/expense';

@Component({
  selector: 'app-expense-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './expense-list.html',
  styleUrl: './expense-list.css',
})
export class ExpenseList implements OnInit {

  expenses: any[] = [];

  filterText = '';
  startDate = '';
  endDate = '';

  readonly router = inject(Router);
  readonly expenseService = inject(ExpenseService);

  ngOnInit(): void {
    const token = typeof localStorage !== 'undefined' ? localStorage.getItem('token') : null;
    if (token) {
      this.expenseService.getExpenses().subscribe({
        next: (response: any) => {
          const data = response?.data ?? response;
          this.expenses = this.sortExpenses(Array.isArray(data) ? data : []);
        },
        error: () => this.load()
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
      if (filter && !(e.description ?? '').toLowerCase().includes(filter)) {return false;}
      if (this.startDate && e.date < this.startDate) {return false;}
      if (this.endDate && e.date > this.endDate) {return false;}
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
  }

  delete(id: number): void {
    this.expenses = this.expenses.filter(e => e.id !== id);
    if (typeof localStorage !== 'undefined') {
      try {
        localStorage.setItem('expenses', JSON.stringify(this.expenses));
      } catch {
        // ignore
      }
    }
  }

  public editExpense(id: string): void{
    this.router.navigate(['/expenses/edit', id]);
  }

}
