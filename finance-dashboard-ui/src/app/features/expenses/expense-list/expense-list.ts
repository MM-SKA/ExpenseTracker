import { Component, inject } from '@angular/core';
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
export class ExpenseList {

  expenses: any[] = [];

  filterText = '';
  startDate = '';
  endDate = '';

  private router = inject(Router);
  private expenseService = inject(ExpenseService);

  ngOnInit(): void {
    const token = typeof localStorage !== 'undefined' ? localStorage.getItem('token') : null;
    if (token) {
      this.expenseService.getExpenses().subscribe({
        next: (response: any) => {
          const data = response?.data ?? response;
          this.expenses = Array.isArray(data) ? data : [];
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
        this.expenses = JSON.parse(localStorage.getItem('expenses') || '[]');
      } catch {
        this.expenses = [];
      }
    } else {
      this.expenses = [];
    }
  }

  filtered(): any[] {
    const filter = this.filterText?.trim().toLowerCase();
    return this.expenses.filter(e => {
      if (filter && !(e.description ?? '').toLowerCase().includes(filter)) return false;
      if (this.startDate && e.date < this.startDate) return false;
      if (this.endDate && e.date > this.endDate) return false;
      return true;
    });
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

}
