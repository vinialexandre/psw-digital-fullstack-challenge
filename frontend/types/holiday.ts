export interface Holiday {
  date: string;
  name: string;
  type: string;
}

export interface HolidayFilter {
  date?: string;
  type?: string;
  searchTerm?: string;
  sortBy?: string;
  sortDescending?: boolean;
}

export interface ApiResponse<T> {
  success: boolean;
  data: T;
  message: string;
  totalRecords: number;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  expiresAt: string;
}

