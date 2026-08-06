import {
  Component,
  inject,
  ChangeDetectorRef,
  OnInit
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';

import { CategoryService }
from '../../../core/services/category.service';

import { Category }
from '../../../shared/models/category/category';

@Component({
  selector: 'app-category-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './category-list.html',
  styleUrl: './category-list.css'
})
export class CategoryList implements OnInit {

  categories: Category[] = [];

  isLoading = true;

  readonly router = inject(Router);

  readonly categoryService =
    inject(CategoryService);

  readonly cdr =
    inject(ChangeDetectorRef);

  ngOnInit(): void {

    console.log('Category API starting');

    this.categoryService
      .getCategories()
      .subscribe({

        next: (response: Category[]) => {

          console.log('Category API success');

          this.categories = response;

          this.isLoading = false;

          console.log(
            'Assigned count:',
            this.categories.length
          );

          this.cdr.detectChanges();

        },

        error: (error) => {

          console.error(error);

          this.isLoading = false;

          this.cdr.detectChanges();

        }

      });

  }

  editCategory(id: string): void {

    this.router.navigate([
      '/categories/edit',
      id
    ]);

  }

}
