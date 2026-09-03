export interface Student {
  id: number;
  name: string;
  gradeCount: number;
  distinctTeacherCount: number;
  totalTeachers: number;
}

export interface CreateStudent {
  name: string;
}

export interface UpdateStudent {
  name: string;
}