import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ExpenseService } from '../../../core/services/expense';
import { CategoryService } from '../../../core/services/category.service';
import { StorageService } from '../../../core/services/storage.service';
import { Category } from '../../../shared/models/category/category';
import { ToastrService } from 'ngx-toastr';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-create-expense',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslatePipe],
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
  private readonly translate = inject(TranslateService);
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

    this.expenseService
      .createExpense({
        categoryId: this.categoryId,
        amount: Number.parseFloat(this.amount) || 0,
        notes: this.description,
        date: this.date,
        location: this.location
      })
      .subscribe({

        next: () => {

          this.toastr.success(
            'Expense created successfully.',
            'Success'
          );

          this.router.navigate(['/expenses']);
        },

        error: (error) => {

          console.error(error);

          this.toastr.error(
            'Unable to create expense.'
          );
        }
      });
  }

}
