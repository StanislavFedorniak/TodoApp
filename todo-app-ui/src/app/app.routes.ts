import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./features/categories/category-list/category-list.component').then(m => m.CategoryListComponent)
  }
];
