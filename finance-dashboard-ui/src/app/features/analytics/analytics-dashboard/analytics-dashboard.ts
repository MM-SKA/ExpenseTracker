import {
  Component,
  inject,
  OnInit,
  ChangeDetectorRef,
  ChangeDetectionStrategy,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ExpenseService } from '../../../core/services/expense';
import { CategoryService } from '../../../core/services/category.service';
import { Category } from '../../../shared/models/category/category';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-analytics-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslatePipe],
  templateUrl: './analytics-dashboard.html',
  changeDetection: ChangeDetectionStrategy.Eager,
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
    notes: '',
  };
  categories: Category[] = [];

  readonly expenseService = inject(ExpenseService);
  readonly categoryService = inject(CategoryService);
  private readonly cdr = inject(ChangeDetectorRef);
  private readonly translate = inject(TranslateService);
  private readonly toastr = inject(ToastrService);

  ngOnInit(): void {
    this.loadCategories();
    this.loadAnalytics();
  }

  loadCategories(): void {
    this.categoryService.getCategories().subscribe({
      next: (response: any) => {
        const data = response?.data ?? response;
        this.categories = Array.isArray(data) ? data : [];
        this.cdr.detectChanges();
      },
      error: () => {
        this.categories = [];
      },
    });
  }

  loadAnalytics(): void {
    this.isLoading = true;
    this.expenseService
      .filterPagedExpenses({
        pageNumber: 1,
        pageSize: 10,
        sortOrder: 'recent',
        includeAnalytics: true,
      })
      .subscribe({
        next: (response: any) => {
          const data = response?.data ?? response;
          this.analytics = data?.analytics ?? data?.Analytics ?? null;
          this.isLoading = false;
          this.cdr.detectChanges();
        },
        error: (error) => {
          this.analytics = null;
          this.isLoading = false;
          this.toastr.error(error?.error?.message ?? 'Unable to load analytics.', 'Analytics');
          this.cdr.detectChanges();
        },
      });
  }

  refresh(): void {
    this.filters = {
      categoryId: '',
      startDate: '',
      endDate: '',
      minAmount: null,
      maxAmount: null,
      notes: '',
    };
    this.loadAnalytics();
  }

  updateAnalytics(): void {
    if (!this.validateFilters()) {
      return;
    }
    this.isLoading = true;
    this.expenseService
      .filterPagedExpenses({
        categoryId: this.filters.categoryId || undefined,
        startDate: this.filters.startDate || undefined,
        endDate: this.filters.endDate || undefined,
        minAmount: this.filters.minAmount ?? undefined,
        maxAmount: this.filters.maxAmount ?? undefined,
        notes: this.filters.notes.trim() || undefined,
        includeAnalytics: true,
        pageNumber: 1,
        pageSize: 10,
        sortOrder: 'recent',
      })
      .subscribe({
        next: (response: any) => {
          const data = response?.data ?? response;
          this.analytics = data?.analytics ?? data?.Analytics ?? null;
          this.isLoading = false;
          this.cdr.detectChanges();
        },
        error: (error) => {
          this.analytics = null;
          this.isLoading = false;
          this.toastr.error(error?.error?.message ?? 'Unable to load analytics.', 'Analytics');
          this.cdr.detectChanges();
        },
      });
  }

  private validateFilters(): boolean {
    if (
      this.filters.startDate &&
      this.filters.endDate &&
      this.filters.startDate > this.filters.endDate
    ) {
      this.toastr.warning('Start date cannot be after end date.', 'Validation');

      return false;
    }

    if (this.filters.minAmount !== null && this.filters.minAmount < 0) {
      this.toastr.warning('Minimum amount cannot be negative.', 'Validation');

      return false;
    }

    if (this.filters.maxAmount !== null && this.filters.maxAmount < 0) {
      this.toastr.warning('Maximum amount cannot be negative.', 'Validation');

      return false;
    }

    if (
      this.filters.minAmount !== null &&
      this.filters.maxAmount !== null &&
      this.filters.minAmount > this.filters.maxAmount
    ) {
      this.toastr.warning('Minimum amount cannot be greater than maximum amount.', 'Validation');

      return false;
    }

    return true;
  }
}
