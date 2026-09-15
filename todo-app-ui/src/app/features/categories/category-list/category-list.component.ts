import {Component, inject, OnInit, signal} from '@angular/core';
import {CategoryService} from '../../../core/services/category.service';
import {Category} from '../../../core/models/category.model';

@Component({
  selector: 'app-category-list',
  templateUrl: './category-list.component.html'
})
export class CategoryListComponent implements OnInit {
  private readonly categoryService = inject(CategoryService);

  readonly categories = signal<Category[]>([]);
  readonly loading = signal<boolean>(false);
  readonly error = signal<string | null>(null);

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
        console.error('Error loading categories:', err)
      }
    })
  }
}
