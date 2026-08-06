import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

import { CategoryService } from '../../../core/services/category.service';

@Component({
  selector: 'app-update-category',
  imports: [FormsModule],
  templateUrl: './update-category.html',
  styleUrl: './update-category.css',
  standalone: true
})
export class UpdateCategory implements OnInit {
  id = '';
  name = '';
  private route =
    inject(ActivatedRoute);

  private router =
    inject(Router);

  private categoryService =
    inject(CategoryService);

  ngOnInit(): void {

    this.id =
      this.route.snapshot.params['id'];

    this.categoryService
      .getCategoryById(this.id)
      .subscribe({

        next: (response: any) => {

          this.name =
            response.name;

        },

        error: console.error

      });

  }

  updateCategory(): void {
    this.categoryService
      .updateCategory(
        this.id,
        {
          name: this.name
        }
      )
      .subscribe({
        next: () => {
          this.categoryService.refreshCategories()
            .subscribe({
              next: () => {
                this.router.navigate(
                  ['/categories']
                );
              }
            });
        },
        error: console.error
      });

  }
}
