import { Component, inject, OnInit, signal } from '@angular/core';
import { ReactiveFormsModule, FormControl, FormGroup, Validators } from '@angular/forms';
import { CategoryService } from '../../../core/services/category.service';
import {Category, CategoryCreateDto} from '../../../core/models/category.model';

@Component({
  selector: 'app-category-list',
  imports: [ReactiveFormsModule],
  templateUrl: './category-list.component.html'
})
export class CategoryListComponent implements OnInit {
  private readonly categoryService = inject(CategoryService);

  readonly categories = signal<Category[]>([]);
  readonly loading = signal<boolean>(false);
  readonly error = signal<string | null>(null);

  readonly isSubmitting = signal<boolean>(false);
  readonly deletingCategoryId = signal<string | null>(null);

  readonly categoryForm = new FormGroup({
    name: new FormControl('', { nonNullable: true, validators: [Validators.required] })
  });

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.loading.set(true);
    this.error.set(null);

    this.categoryService.getAll().subscribe({
      next: (data: Category[]) => {
        this.categories.set(data);
        this.loading.set(false);
      },
      error: (err: unknown) => {
        this.error.set('Failed to load categories.');
        this.loading.set(false);
        console.error('Error loading categories:', err);
      }
    });
  }

  addCategory(): void {
    if (this.categoryForm.invalid) return;

    this.isSubmitting.set(true);
    this.error.set(null);

    const newCategory: CategoryCreateDto = { name: this.categoryForm.controls.name.value };

    this.categoryService.create(newCategory).subscribe({
      next: (createdCategory: Category) => {
        this.categories.update(cats => [...cats, createdCategory]);
        this.categoryForm.reset();
        this.isSubmitting.set(false);
      },
      error: (err: unknown) => {
        this.error.set('Failed to create category.');
        this.isSubmitting.set(false);
        console.error('Error creating category:', err);
      }
    });
  }

  deleteCategory(id: string): void {
    this.deletingCategoryId.set(id);
    this.error.set(null);

    this.categoryService.delete(id).subscribe({
      next: () => {
        this.categories.update(cats => cats.filter(c => c.id !== id));
        this.deletingCategoryId.set(null);
      },
      error: (err: unknown) => {
        this.error.set('Failed to delete category.');
        this.deletingCategoryId.set(null);
        console.error('Error deleting category:', err);
      }
    });
  }
}
