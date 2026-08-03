import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ExpenseService } from '../../../core/services/expense';
import { CategoryService } from '../../../core/services/category.service';
import { Category } from '../../../shared/models/category/category';

@Component({
  selector: 'app-analytics-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './analytics-dashboard.html',
  styleUrl: './analytics-dashboard.css',
})
export class AnalyticsDashboard implements OnInit {
  analytics: any = null;
  isLoading = false;
  filters = {
    categoryId: '',
    startDate: '',
    endDate: '',
    minAmount: null as number | null,
    maxAmount: null as number | null,
    notes: ''
  };
  categories: Category[] = [];

  readonly expenseService = inject(ExpenseService);
  readonly categoryService = inject(CategoryService);

  ngOnInit(): void {
    this.loadCategories();
    this.loadAnalytics();
  }

  loadCategories(): void {
    this.categoryService.getCategories().subscribe({
      next: (response: any) => {
        const data = response?.data ?? response;
        this.categories = Array.isArray(data) ? data : [];
      },
      error: () => {
        this.categories = [];
      }
    });
  }

  loadAnalytics(): void {
    this.isLoading = true;
    this.expenseService.filterExpenses({ includeAnalytics: true }).subscribe({
      next: (response: any) => {
        const data = response?.data ?? response;
        this.analytics = data?.analytics ?? data?.Analytics ?? null;
        this.isLoading = false;
      },
      error: () => {
        this.analytics = null;
        this.isLoading = false;
      }
    });
  }

  refresh(): void {
    this.filters = {
      categoryId: '',
      startDate: '',
      endDate: '',
      minAmount: null,
      maxAmount: null,
      notes: ''
    };
    this.loadAnalytics();
  }

  updateAnalytics(): void {
    this.isLoading = true;
    this.expenseService.filterExpenses({
      categoryId: this.filters.categoryId || undefined,
      startDate: this.filters.startDate || undefined,
      endDate: this.filters.endDate || undefined,
      minAmount: this.filters.minAmount ?? undefined,
      maxAmount: this.filters.maxAmount ?? undefined,
      notes: this.filters.notes || undefined,
      includeAnalytics: true
    }).subscribe({
      next: (response: any) => {
        const data = response?.data ?? response;
        this.analytics = data?.analytics ?? data?.Analytics ?? null;
        this.isLoading = false;
      },
      error: () => {
        this.analytics = null;
        this.isLoading = false;
      }
    });
  }
}
