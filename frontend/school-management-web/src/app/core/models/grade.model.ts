export interface Grade {
  id: number;
  name: string;
  value: number;
  studentId: number;
  studentName: string;
  teacherId: number;
  teacherName: string;
}

export interface CreateGrade {
  name: string;
  value: number;
  studentId: number;
  teacherId: number;
}

export interface UpdateGrade {
  name: string;
  value: number;
  studentId: number;
  teacherId: number;
}