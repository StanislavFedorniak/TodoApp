export interface TodoTask {
  id: string;
  title: string;
  description: string | null;
  isCompleted: boolean;
  categoryId: string | null;
}

export interface TodoTaskCreateDto {
  title: string;
  description?: string | null;
  categoryId?: string | null;
}

export interface TodoTaskUpdateDto {
  title: string;
  description?: string | null;
  isCompleted: boolean;
  categoryId?: string | null;
}

