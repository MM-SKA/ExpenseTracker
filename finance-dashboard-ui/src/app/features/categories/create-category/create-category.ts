import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CategoryService } from '../../../core/services/category.service';
import { CommonModule } from '@angular/common';
import { ToastrService } from 'ngx-toastr';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-create-category',
  imports: [FormsModule, CommonModule, TranslatePipe],
  standalone: true,
  templateUrl: './create-category.html',
  styleUrl: './create-category.css',
})
export class CreateCategory {
  name = '';
  private readonly categoryService = inject(CategoryService);
  private readonly router = inject(Router);
  private readonly toastr = inject(ToastrService);
  private readonly translate = inject(TranslateService);

  createCategory(): void {
    this.categoryService.createCategory({
      name: this.name
    }).subscribe({
      next: () => {
        this.toastr.success('Category created successfully.', 'Success');
        this.router.navigate(['/categories']);
      },
      error: console.error
    })
  }
}
