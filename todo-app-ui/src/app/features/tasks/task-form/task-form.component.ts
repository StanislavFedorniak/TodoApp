import {Component, inject, input, output, signal} from '@angular/core';
import {FormControl, FormGroup, ReactiveFormsModule} from '@angular/forms';
import {finalize} from 'rxjs';
import {TodoTaskService} from '../../../core/services/todo-task.service';
import {TodoTask, TodoTaskCreateDto} from '../../../core/models/task.model';
import {Category} from '../../../core/models/category.model';
import {TITLE_VALIDATORS} from '../task-title.validators';

@Component({
  selector: 'app-task-form',
  imports: [ReactiveFormsModule],
  templateUrl: './task-form.component.html'
})
export class TaskFormComponent {
  private readonly todoTaskService = inject(TodoTaskService);

  readonly categories = input.required<Category[]>();
  readonly created = output<TodoTask>();

  readonly isSubmitting = signal<boolean>(false);
  readonly error = signal<string | null>(null);

  readonly taskForm = new FormGroup({
    title: new FormControl('', { nonNullable: true, validators: TITLE_VALIDATORS }),
    description: new FormControl<string | null>(null),
    categoryId: new FormControl<string | null>(null)
  });

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
          this.created.emit(createdTask);
          this.taskForm.reset();
        },
        error: (err: unknown) => {
          this.error.set('Failed to create task.');
          console.error('Error creating task:', err);
        }
      });
  }
}
