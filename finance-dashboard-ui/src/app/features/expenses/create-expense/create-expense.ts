import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ExpenseService } from '../../../core/services/expense';
import { CategoryService } from '../../../core/services/category.service';
import { StorageService } from '../../../core/services/storage.service';
import { Category } from '../../../shared/models/category/category';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-create-expense',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './create-expense.html',
  styleUrl: './create-expense.css',
})
export class CreateExpense implements OnInit {

  description = '';
  amount = '';
  date = '';
  location = '';
  isLoadingCategories = true;

  readonly router = inject(Router);
  readonly expenseService = inject(ExpenseService);
  readonly categoryService = inject(CategoryService);
  private readonly storageService = inject(StorageService);
  private readonly toastr = inject(ToastrService);
  categoryId = '';
  categories: Category[] = [];

  ngOnInit(): void {
    this.loadCategories();
  }

  private loadCategories(): void {
    this.isLoadingCategories = true;
    this.categoryService.getCategories().subscribe({
      next: (response: any) => {
        const data = response?.data ?? response;
        this.categories = Array.isArray(data) ? data : [];
        this.categoryId = this.categories.length ? this.categories[0].id ?? '' : '';
        this.isLoadingCategories = false;
      },
      error: () => {
        this.categories = [];
        this.isLoadingCategories = false;
      }
    });
  }

  save(): void {
    const token = typeof localStorage !== 'undefined' ? localStorage.getItem('token') : null;
    if (token) {
      this.expenseService.createExpense({
        categoryId: this.categoryId,
        amount: Number.parseFloat(this.amount) || 0,
        notes: this.description,
        date: this.date,
        location: this.location
      }).subscribe({
        next: () => {
          this.toastr.success('Expense created successfully.', 'Success');
          this.router.navigate(['/expenses'])
        },
        error: () => this.saveToLocal()
      });
    } else {
      this.saveToLocal();
    }
  }

  private saveToLocal(): void {
    const list = this.storageService.getEncrypted<any[]>('expenses') || [];
    list.push({
      id: Date.now().toString(),
      description: this.description,
      amount: Number.parseFloat(this.amount) || 0,
      date: this.date,
      location: this.location,
      categoryName: this.categories.find(c => c.id === this.categoryId)?.name || 'General'
    });
    this.storageService.setEncrypted('expenses', list);
    this.router.navigate(['/expenses']);
  }

}
