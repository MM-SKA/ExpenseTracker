import {
  Component,
  inject,
  ChangeDetectorRef,
  OnInit,
  ChangeDetectionStrategy,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { CategoryService } from '../../../core/services/category.service';
import { Category } from '../../../shared/models/category/category';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-category-list',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslatePipe],
  templateUrl: './category-list.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './category-list.css',
})
export class CategoryList implements OnInit {
  categories: Category[] = [];
  isLoading = true;
  readonly router = inject(Router);
  readonly categoryService = inject(CategoryService);
  readonly cdr = inject(ChangeDetectorRef);
  private readonly translate = inject(TranslateService);
  private readonly toastr = inject(ToastrService);

  ngOnInit(): void {
    console.log('Category API starting');

    this.categoryService.getCategories().subscribe({
      next: (response: Category[]) => {
        console.log('Category API success');

        this.categories = response;

        this.isLoading = false;

        console.log('Assigned count:', this.categories.length);

        this.cdr.detectChanges();
      },

      error: (error) => {
        console.error(error);

        this.isLoading = false;

        this.cdr.detectChanges();
      },
    });
  }

  viewCategoryExpenses(categoryName?: string): void {
    if (!categoryName) return;
    this.router.navigate(['/expenses'], { queryParams: { category: categoryName } });
  }

  editCategory(id: string): void {
    this.router.navigate(['/categories/edit', id]);
  }

  deleteCategory(id: string): void {
    if (!confirm('Are you sure you want to delete this category?')) {
      return;
    }

    this.categoryService.deleteCategory(id).subscribe({
      next: () => {
        this.toastr.success('Category deleted successfully.', 'Success');

        this.categoryService.getCategories(true).subscribe({
          next: (categories) => {
            this.categories = categories;

            this.cdr.detectChanges();
          },
        });
      },

      error: (error) => {
        console.error(error);

        this.toastr.error('Unable to delete category.', 'Delete Failed');
      },
    });
  }
}
