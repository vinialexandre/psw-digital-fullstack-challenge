import axios from 'axios';
import type { ApiResponse, Holiday, HolidayFilter, LoginRequest, LoginResponse } from '@/types/holiday';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export const authService = {
  async login(credentials: LoginRequest): Promise<ApiResponse<LoginResponse>> {
    const response = await api.post<ApiResponse<LoginResponse>>('/auth/login', credentials);
    return response.data;
  },
};

export const holidayService = {
  async getHolidays(filter?: HolidayFilter): Promise<ApiResponse<Holiday[]>> {
    const params = new URLSearchParams();
    
    if (filter?.date) params.append('date', filter.date);
    if (filter?.type) params.append('type', filter.type);
    if (filter?.searchTerm) params.append('searchTerm', filter.searchTerm);
    if (filter?.sortBy) params.append('sortBy', filter.sortBy);
    if (filter?.sortDescending !== undefined) params.append('sortDescending', filter.sortDescending.toString());

    const response = await api.get<ApiResponse<Holiday[]>>(`/holidays?${params.toString()}`);
    return response.data;
  },
};

