import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {Component, inject, signal} from '@angular/core';
import {TodoTaskService} from '../../../core/services/todo-task.service';
import {TodoTask, TodoTaskCreateDto, TodoTaskUpdateDto} from '../../../core/models/task.model';
import {Category} from '../../../core/models/category.model';
import {CategoryService} from '../../../core/services/category.service';
import {finalize} from 'rxjs';

@Component ({
  selector: 'app-task-list',
  imports: [ReactiveFormsModule],
  templateUrl: './task-list.component.html'
})
export class TaskListComponent {
  private readonly todoTaskService = inject(TodoTaskService);
  private readonly categoryService = inject(CategoryService);

  readonly tasks = signal<TodoTask[]>([]);
  readonly categories = signal<Category[]>([]);

  readonly loading = signal<boolean>(false);
  readonly error = signal<string | null>(null);
  readonly isSubmitting = signal<boolean>(false);
  readonly trackingTaskId = signal<string | null>(null);

  readonly taskForm = new FormGroup({
    title: new FormControl('', { nonNullable: true, validators: [Validators.required]}),
    description: new FormControl<string | null>(null),
    categoryId: new FormControl<string | null>(null)
  });

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loading.set(true);
    this.error.set(null);

    this.categoryService.getAll().subscribe({
      next: (cats) => this.categories.set(cats),
      error: (err: unknown) => console.error('Failed to load categories', err)
    })

    this.todoTaskService.getAll().subscribe({
      next: (tasks) => {
        this.tasks.set(tasks);
        this.loading.set(false);
      },
      error: (err: unknown) => {
        this.error.set('Failed to load tasks.');
        this.loading.set(false);
        console.error('Error loading tasks', err);
      }
    })
  }

  addTask(): void {
    if (this.taskForm.invalid) return;

    this.isSubmitting.set(true);
    this.error.set(null);

    const formValue = this.taskForm.getRawValue();
    const newTask: TodoTaskCreateDto = {
      title: formValue.title,
      description: formValue.description || undefined,
      categoryId: formValue.categoryId || undefined
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
    }
  }

