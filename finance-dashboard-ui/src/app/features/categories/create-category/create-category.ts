import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CategoryService } from '../../../core/services/category.service';

@Component({
  selector: 'app-create-category',
  imports: [FormsModule],
  standalone: true,
  templateUrl: './create-category.html',
  styleUrl: './create-category.css',
})
export class CreateCategory {
  name = '';
  private categoryService = inject(CategoryService);
  private router = inject(Router);

  createCategory(): void {
    this.categoryService.createCategory({
      name: this.name
    }).subscribe({
      next: () => {
        this.categoryService.refreshCategories()
          .subscribe({
            next: () => {
              this.router.navigate(['/categories']);
            }
          });
      },
      error: console.error
    })
  }
}
