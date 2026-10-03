import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {Component, computed, inject, OnInit, signal} from '@angular/core';
import {TodoTaskService} from '../../../core/services/todo-task.service';
import {TodoTask, TodoTaskCreateDto, TodoTaskUpdateDto} from '../../../core/models/task.model';
import {Category} from '../../../core/models/category.model';
import {CategoryService} from '../../../core/services/category.service';
import {finalize, forkJoin} from 'rxjs';

const TITLE_VALIDATORS = [Validators.required, Validators.pattern(/\S/)];
@Component ({
  selector: 'app-task-list',
  imports: [ReactiveFormsModule],
  templateUrl: './task-list.component.html'
})
export class TaskListComponent implements OnInit{
  private readonly todoTaskService = inject(TodoTaskService);
  private readonly categoryService = inject(CategoryService);

  readonly tasks = signal<TodoTask[]>([]);
  readonly categories = signal<Category[]>([]);

  private readonly categoryNames = computed(
    () => new Map(this.categories().map(c => [c.id, c.name]))
  );

  readonly loading = signal<boolean>(false);
  readonly error = signal<string | null>(null);
  readonly isSubmitting = signal<boolean>(false);
  readonly trackingTaskId = signal<string | null>(null);
  readonly editingTaskId = signal<string | null>(null);

  readonly taskForm = new FormGroup({
    title: new FormControl('', { nonNullable: true, validators: TITLE_VALIDATORS}),
    description: new FormControl<string | null>(null),
    categoryId: new FormControl<string | null>(null)
  });

  readonly editForm = new FormGroup({
    title: new FormControl('', { nonNullable: true, validators: TITLE_VALIDATORS}),
    description: new FormControl<string | null>(null),
    categoryId: new FormControl<string | null>(null)
  });

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loading.set(true);
    this.error.set(null);

    forkJoin({
      categories: this.categoryService.getAll(),
      tasks: this.todoTaskService.getAll()
    })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: ({ categories, tasks }) => {
          this.categories.set(categories);
          this.tasks.set(tasks);
        },
        error: (err: unknown) => {
          this.error.set('Failed to load data.');
          console.error('Error loading data', err);
        }
    })
  }

  categoryName(categoryId: string | null): string | null {
     if (!categoryId) return null;
     return this.categoryNames().get(categoryId) ?? null;
  }

  addTask(): void {
    if (this.taskForm.invalid) return;

    this.isSubmitting.set(true);
    this.error.set(null);

    const formValue = this.taskForm.getRawValue();
    const newTask: TodoTaskCreateDto = {
      title: formValue.title.trim(),
      description: formValue.description?.trim() || null,
      categoryId: formValue.categoryId
    };

    this.todoTaskService.create(newTask)
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: (createdTask: TodoTask) => {
          this.tasks.update(tasks => [...tasks, createdTask]);
          this.taskForm.reset();
        },
        error: (err: unknown) => {
          this.error.set('Failed to create task.');
          console.error('Error creating task:', err);
        }
      });
  }

  toggleStatus(task: TodoTask): void {
    this.trackingTaskId.set(task.id);
    this.error.set(null);

    const updateDto: TodoTaskUpdateDto = {
      title: task.title,
      description: task.description,
      categoryId: task.categoryId,
      isCompleted: !task.isCompleted,
    };

    this.todoTaskService.update(task.id, updateDto)
      .pipe(finalize(() => this.trackingTaskId.set(null)))
      .subscribe({
        next: () => {
          this.tasks.update(tasks =>
            tasks.map(t => (t.id === task.id ? { ...t, isCompleted: updateDto.isCompleted } : t))
          );
        },
        error: (err: unknown) => {
          this.error.set('Failed to update task status');
          console.error('Error updating task status', err);
        }
      });
  }

  startEdit(task: TodoTask): void {
    this.editingTaskId.set(task.id);
    this.editForm.setValue({
      title: task.title,
      description: task.description || null,
      categoryId: task.categoryId || null
    });
  }

  cancelEdit(): void {
    this.editingTaskId.set(null);
    this.editForm.reset();
  }

  saveEdit(task: TodoTask): void {
    if (this.editForm.invalid) return;

    this.trackingTaskId.set(task.id);
    this.error.set(null);

    const formValue = this.editForm.getRawValue();
    const updateDto: TodoTaskUpdateDto = {
      title: formValue.title.trim(),
      description: formValue.description?.trim() || null,
      isCompleted: task.isCompleted,
      categoryId: formValue.categoryId
    };

    this.todoTaskService.update(task.id, updateDto)
      .pipe(finalize(() => this.trackingTaskId.set(null)))
      .subscribe({
        next: () => {
          this.tasks.update(tasks =>
            tasks.map(t => (t.id === task.id ? {...t, ...updateDto} : t))
          );
          this.cancelEdit();
        },
        error: (err: unknown) => {
          this.error.set('Failed to update task');
          console.error('Error updating task', err);
        }
      })
  }

  deleteTask(taskId: string) : void {
    if (!confirm('Are you confirm you want to delete this task?')) return;

    this.trackingTaskId.set(taskId);
    this.error.set(null);

    this.todoTaskService.delete(taskId)
      .pipe(finalize(() => this.trackingTaskId.set(null)))
      .subscribe({
        next: () => {
          this.tasks.update(tasks => tasks.filter(t => t.id !== taskId));
        },
        error: (err: unknown)=> {
          this.error.set('Failed to delete task.');
          console.error('Error deleting task', err);
        }
      });
  }
}

