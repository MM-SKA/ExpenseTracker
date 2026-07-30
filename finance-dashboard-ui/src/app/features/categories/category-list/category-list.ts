import { Component, inject } from "@angular/core";
import { CategoryService } from "../../../core/services/category.service";
import { Category } from "../../../shared/models/category/category";

@Component({
  selector:'app-category-list',
  templateUrl:'./category-list.html',
  styleUrl: './category-list.css',
  standalone:true
})
export class CategoryList{

  constructor() {
    console.log('Category Component Constructed');
  }
  categories: Category[] = [];

  private categoryService = inject(CategoryService);
  ngOnInit():void{
    console.log('Category API starting');
    this.categoryService.getCategories().subscribe({
      next:(response)=>{
        console.log('Category API success');
        this.categories=response;
        console.log(response);
      },
      error:console.error
    });
  }
}
