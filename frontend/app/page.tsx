'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { DataTable, Column } from '@/components/DataTable';
import { HolidayFilters } from '@/components/HolidayFilters';
import { useHolidays } from '@/hooks/useHolidays';
import { authService } from '@/lib/api';
import type { Holiday, HolidayFilter } from '@/types/holiday';

export default function HomePage() {
  const router = useRouter();
  const [mounted, setMounted] = useState(false);
  const currentYear = new Date().getFullYear();

  const allHolidays = useHolidays({ year: currentYear, sortBy: 'date' });

  useEffect(() => {
    setMounted(true);
  }, []);

  const handleLogout = async () => {
    try {
      await authService.logout();
    } catch (error) {
      console.error('Logout error:', error);
    } finally {
      router.push('/login');
    }
  };

  const handleSort = (field: keyof Holiday, descending: boolean) => {
    allHolidays.setFilter({
      ...allHolidays.filter,
      sortBy: field as string,
      sortDescending: descending,
    });
  };

  const handleFilterChange = (filter: HolidayFilter) => {
    allHolidays.setFilter({
      ...allHolidays.filter,
      ...filter,
    });
  };

  const columns: Column<Holiday>[] = [
    {
      key: 'date',
      header: 'Data',
      sortable: true,
    },
    {
      key: 'name',
      header: 'Nome',
      sortable: true,
    },
    {
      key: 'type',
      header: 'Tipo',
      sortable: true,
      render: (value) => (
        <span
          className={`px-2 py-1 rounded-full text-xs font-medium ${
            value === 'National'
              ? 'bg-blue-100 text-blue-800'
              : 'bg-green-100 text-green-800'
          }`}
        >
          {value === 'National' ? 'Nacional' : 'Municipal'}
        </span>
      ),
    },
  ];



  if (!mounted) {
    return null;
  }

  const displayYear = allHolidays.filter.year || currentYear;

  return (
    <div className="min-h-screen bg-gray-100">
      <header className="bg-white shadow">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 flex justify-between items-center">
          <h1 className="text-2xl font-bold text-gray-900">Feriados Brasileiros {displayYear}</h1>
          <button
            onClick={handleLogout}
            className="px-4 py-2 bg-red-600 text-white rounded-md hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-red-500 flex items-center gap-2"
          >
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" className="w-5 h-5">
              <path strokeLinecap="round" strokeLinejoin="round" d="M15.75 9V5.25A2.25 2.25 0 0013.5 3h-6a2.25 2.25 0 00-2.25 2.25v13.5A2.25 2.25 0 007.5 21h6a2.25 2.25 0 002.25-2.25V15m3 0l3-3m0 0l-3-3m3 3H9" />
            </svg>
            Sair
          </button>
        </div>
      </header>

      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <HolidayFilters onFilterChange={handleFilterChange} />

        {allHolidays.loading && (
          <div className="text-center py-8">
            <p className="text-gray-600">Carregando feriados...</p>
          </div>
        )}

        {allHolidays.error && (
          <div className="p-4 bg-red-50 border border-red-200 rounded-md mb-4">
            <p className="text-red-600">{allHolidays.error}</p>
          </div>
        )}

        {!allHolidays.loading && !allHolidays.error && allHolidays.holidays.length === 0 && (
          <div className="text-center py-8">
            <p className="text-gray-600">Nenhum dado encontrado</p>
          </div>
        )}

        {!allHolidays.loading && !allHolidays.error && allHolidays.holidays.length > 0 && (
          <>
            <div className="mb-4 text-sm text-gray-600">
              Total de registros: <span className="font-semibold">{allHolidays.totalRecords}</span>
            </div>

            <DataTable
              data={allHolidays.holidays}
              columns={columns}
              onSort={handleSort}
              currentSortField={allHolidays.filter.sortBy as keyof Holiday}
              currentSortDescending={allHolidays.filter.sortDescending}
            />
          </>
        )}
      </main>
    </div>
  );
}

