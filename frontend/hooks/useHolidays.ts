import { useState, useEffect, useCallback } from 'react';
import { holidayService } from '@/lib/api';
import type { Holiday, HolidayFilter } from '@/types/holiday';

export function useHolidays(initialFilter?: HolidayFilter) {
  const [holidays, setHolidays] = useState<Holiday[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [totalRecords, setTotalRecords] = useState(0);
  const [filter, setFilter] = useState<HolidayFilter>(initialFilter || {});

  const fetchHolidays = useCallback(async () => {
    setLoading(true);
    setError(null);
    
    try {
      const response = await holidayService.getHolidays(filter);
      if (response.success) {
        setHolidays(response.data);
        setTotalRecords(response.totalRecords);
      } else {
        setError(response.message);
      }
    } catch (err: unknown) {
      if (err && typeof err === 'object' && 'response' in err) {
        const axiosError = err as { response?: { status?: number } };
        if (axiosError.response?.status === 401) {
          return;
        } else if (axiosError.response) {
          setError('Erro ao carregar feriados');
        } else {
          return;
        }
      } else {
        return;
      }
    } finally {
      setLoading(false);
    }
  }, [filter]);

  useEffect(() => {
    fetchHolidays();
  }, [fetchHolidays]);

  return {
    holidays,
    loading,
    error,
    totalRecords,
    filter,
    setFilter,
    refetch: fetchHolidays,
  };
}

