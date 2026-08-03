import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ExpenseService } from '../../../core/services/expense';
import { CategoryService } from '../../../core/services/category.service';
import { Category } from '../../../shared/models/category/category';

@Component({
  selector: 'app-create-expense',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './create-expense.html',
  styleUrl: './create-expense.css',
})
export class CreateExpense {

  description = '';
  amount = '';
  date = '';

  private router = inject(Router);
  private expenseService = inject(ExpenseService);
  private categoryService = inject(CategoryService);
  categoryId = '';
  categories: Category[] = [];

  ngOnInit(): void {
    this.categoryService.getCategories().subscribe({
      next: (response: any) => {
        const data = response?.data ?? response;
        this.categories = Array.isArray(data) ? data : [];
        if (this.categories.length) this.categoryId = this.categories[0].id ?? this.categories[0].id ?? '';
      },
      error: () => (this.categories = [])
    });
  }

  save(): void {
    const token = typeof localStorage !== 'undefined' ? localStorage.getItem('token') : null;
    if (token) {
      this.expenseService.createExpense({
        categoryId: this.categoryId,
        amount: parseFloat(this.amount) || 0,
        notes: this.description,
        date: this.date,
        location: ''
      }).subscribe({ next: () => this.router.navigate(['/expenses']), error: () => this.saveToLocal() });
    } else {
      this.saveToLocal();
    }
  }

  private saveToLocal(): void {
    let list: any[] = [];
    if (typeof localStorage !== 'undefined') {
      try {
        list = JSON.parse(localStorage.getItem('expenses') || '[]');
      } catch {
        list = [];
      }
      list.push({ id: Date.now(), description: this.description, amount: parseFloat(this.amount) || 0, date: this.date });
      try {
        localStorage.setItem('expenses', JSON.stringify(list));
      } catch {
        // ignore
      }
    }
    this.router.navigate(['/expenses']);
  }

}
