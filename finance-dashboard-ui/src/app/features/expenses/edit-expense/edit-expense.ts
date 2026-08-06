import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ExpenseService } from '../../../core/services/expense';
import { CategoryService } from '../../../core/services/category.service';
import { Category } from '../../../shared/models/category/category';

@Component({
  selector: 'app-edit-expense',
  imports: [CommonModule, FormsModule],
  templateUrl: './edit-expense.html',
  styleUrl: './edit-expense.css',
})
export class EditExpense implements OnInit{
  id = '';
  notes = '';
  amount = 0;
  categoryId = '';
  date = '';
  location = '';
  categories: Category[] = [];

  private readonly expenseService = inject(ExpenseService);
  private readonly categoryService = inject(CategoryService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  ngOnInit(): void {
    this.id = this.route.snapshot.params['id'];
    this.loadCategories();
    this.loadExpense();
  }

  loadCategories(): void {

    this.categoryService
      .getCategories()
      .subscribe({
        next: (response: Category[]) => {
          this.categories = response;
        }
      });
  }

  loadExpense(): void {
    this.expenseService
      .getExpenseById(this.id)
      .subscribe({
        next: (response: any) => {
          this.notes = response.notes;
          this.amount = response.amount;
          this.categoryId = response.categoryId;
          this.location = response.location ?? '';
          this.date = response.date?.split('T')[0];
        }
      });
  }

  updateExpense(): void {
    this.expenseService.updateExpense(this.id,
        {
          notes: this.notes,
          amount: this.amount,
          categoryId: this.categoryId,
          date: this.date,
          location: this.location
        })
      .subscribe({
        next: () => {
          this.router.navigate(
            ['/expenses']
          );
        }
      });
  }
}
