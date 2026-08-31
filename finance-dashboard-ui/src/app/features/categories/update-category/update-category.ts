import { Component, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { CategoryService } from '../../../core/services/category.service';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-update-category',
  imports: [FormsModule, TranslatePipe],
  templateUrl: './update-category.html',
  styleUrl: './update-category.css',
  changeDetection: ChangeDetectionStrategy.Eager,
  standalone: true,
})
export class UpdateCategory implements OnInit {
  id = '';
  name = '';

  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly categoryService = inject(CategoryService);
  private readonly toastr = inject(ToastrService);
  private readonly translate = inject(TranslateService);

  ngOnInit(): void {
    this.id = this.route.snapshot.params['id'];

    this.categoryService.getCategoryById(this.id).subscribe({
      next: (response: any) => {
        this.name = response.name;
      },

      error: console.error,
    });
  }

  updateCategory(): void {
    if (!this.name?.trim()) {
      this.toastr.warning('Category name is required', 'Validation');
      return;
    }
    this.categoryService
      .updateCategory(this.id, {
        name: this.name,
      })
      .subscribe({
        next: () => {
          this.toastr.success('Category edited successfully.', 'Success');
          this.router.navigate(['/categories']);
        },
        error: console.error,
      });
  }
}
