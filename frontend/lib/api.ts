import axios from 'axios';
import type { ApiResponse, Holiday, HolidayFilter, LoginRequest, LoginResponse } from '@/types/holiday';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5129/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: true,
});

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (typeof globalThis.window !== 'undefined' && !globalThis.window.location.pathname.includes('/login')) {
      if (error.response?.status === 401 || !error.response) {
        globalThis.window.location.href = '/login';
      }
    }
    return Promise.reject(error);
  }
);

export const authService = {
  async login(credentials: LoginRequest): Promise<ApiResponse<LoginResponse>> {
    const response = await api.post<ApiResponse<LoginResponse>>('/auth/login', credentials);
    return response.data;
  },

  async logout(): Promise<void> {
    await api.post('/auth/logout');
  },
};

export const holidayService = {
  async getHolidays(filter?: HolidayFilter): Promise<ApiResponse<Holiday[]>> {
    const params = new URLSearchParams();

    if (filter?.year) params.append('year', filter.year.toString());
    if (filter?.date) params.append('date', filter.date);
    if (filter?.type) params.append('type', filter.type);
    if (filter?.searchTerm) params.append('searchTerm', filter.searchTerm);
    if (filter?.sortBy) params.append('sortBy', filter.sortBy);
    if (filter?.sortDescending !== undefined) params.append('sortDescending', filter.sortDescending.toString());

    const response = await api.get<ApiResponse<Holiday[]>>(`/holidays?${params.toString()}`);
    return response.data;
  },
};

