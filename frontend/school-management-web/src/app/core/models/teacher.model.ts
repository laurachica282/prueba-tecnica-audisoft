export interface Teacher {
  id: number;
  name: string;
  gradeCount: number;
}

export interface CreateTeacher {
  name: string;
}

export interface UpdateTeacher {
  name: string;
}