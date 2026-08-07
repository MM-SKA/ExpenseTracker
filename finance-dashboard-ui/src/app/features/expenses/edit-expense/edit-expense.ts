import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ExpenseService } from '../../../core/services/expense';
import { CategoryService } from '../../../core/services/category.service';
import { Category } from '../../../shared/models/category/category';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-edit-expense',
  standalone:true,
  imports: [CommonModule, FormsModule],
  templateUrl: './edit-expense.html',
  styleUrl: './edit-expense.css',
})
export class EditExpense implements OnInit {
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
  private readonly toastr = inject(ToastrService);
  private readonly cdr = inject(ChangeDetectorRef);

  ngOnInit(): void {
    this.id = this.route.snapshot.params['id'];
    this.loadCategories();
  }

  loadCategories(): void {

    this.categoryService
      .getCategories()
      .subscribe({
        next: (response: Category[]) => {
          this.categories = response;
          this.loadExpense();
        }
      });
  }

  loadExpense(): void {
    this.expenseService
      .getExpenseById(this.id)
      .subscribe({
        next: (response: any) => {
          console.log(response);
          const expense = response.data;
          this.notes = expense.description;
          this.amount = expense.amount;
          this.categoryId = expense.categoryId;
          this.location = expense.location ?? '';
          this.date = expense.date?.split('T')[0];
          console.log('notes', this.notes);
          console.log('amount', this.amount);
          console.log('categoryId', this.categoryId);
          console.log('location', this.location);
          console.log('date', this.date);
          this.cdr.detectChanges();
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
          this.toastr.success('Expense edited successfully.', 'Success');
          this.router.navigate(
            ['/expenses']
          );
        }
      });
  }
}
