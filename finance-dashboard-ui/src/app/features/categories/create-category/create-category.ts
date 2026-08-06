import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CategoryService } from '../../../core/services/category.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-create-category',
  imports: [FormsModule, CommonModule],
  standalone: true,
  templateUrl: './create-category.html',
  styleUrl: './create-category.css',
})
export class CreateCategory {
  name = '';
  private readonly categoryService = inject(CategoryService);
  private readonly router = inject(Router);

  createCategory(): void {
    this.categoryService.createCategory({
      name: this.name
    }).subscribe({
      next: () => {
        this.router.navigate(['/categories']);
      },
      error: console.error
    })
  }
}
