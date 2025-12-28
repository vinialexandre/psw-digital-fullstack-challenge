'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { Tabs } from '@/components/Tabs';
import { DataTable, Column } from '@/components/DataTable';
import { HolidayFilters } from '@/components/HolidayFilters';
import { useHolidays } from '@/hooks/useHolidays';
import type { Holiday, HolidayFilter } from '@/types/holiday';

export default function HomePage() {
  const router = useRouter();
  const [mounted, setMounted] = useState(false);
  
  const allHolidays = useHolidays({ sortBy: 'date' });
  const nationalHolidays = useHolidays({ type: 'National', sortBy: 'date' });
  const municipalHolidays = useHolidays({ type: 'Municipal', sortBy: 'date' });

  useEffect(() => {
    setMounted(true);
    const token = localStorage.getItem('token');
    if (!token) {
      router.push('/login');
    }
  }, [router]);

  const handleLogout = () => {
    localStorage.removeItem('token');
    router.push('/login');
  };

  const handleSort = (hookInstance: ReturnType<typeof useHolidays>) => (
    field: keyof Holiday,
    descending: boolean
  ) => {
    hookInstance.setFilter({
      ...hookInstance.filter,
      sortBy: field,
      sortDescending: descending,
    });
  };

  const handleFilterChange = (hookInstance: ReturnType<typeof useHolidays>) => (
    filter: HolidayFilter
  ) => {
    hookInstance.setFilter({
      ...hookInstance.filter,
      ...filter,
    });
  };

  const columns: Column<Holiday>[] = [
    {
      key: 'date',
      header: 'Date',
      sortable: true,
    },
    {
      key: 'name',
      header: 'Name',
      sortable: true,
    },
    {
      key: 'type',
      header: 'Type',
      sortable: true,
      render: (value) => (
        <span
          className={`px-2 py-1 rounded-full text-xs font-medium ${
            value === 'National'
              ? 'bg-blue-100 text-blue-800'
              : 'bg-green-100 text-green-800'
          }`}
        >
          {String(value)}
        </span>
      ),
    },
  ];

  const renderHolidayTab = (hookInstance: ReturnType<typeof useHolidays>) => (
    <div>
      <HolidayFilters onFilterChange={handleFilterChange(hookInstance)} />
      
      {hookInstance.loading && (
        <div className="text-center py-8">
          <p className="text-gray-600">Loading holidays...</p>
        </div>
      )}

      {hookInstance.error && (
        <div className="p-4 bg-red-50 border border-red-200 rounded-md mb-4">
          <p className="text-red-600">{hookInstance.error}</p>
        </div>
      )}

      {!hookInstance.loading && !hookInstance.error && (
        <>
          <div className="mb-4 text-sm text-gray-600">
            Total records: <span className="font-semibold">{hookInstance.totalRecords}</span>
          </div>
          
          <DataTable
            data={hookInstance.holidays}
            columns={columns}
            onSort={handleSort(hookInstance)}
            currentSortField={hookInstance.filter.sortBy as keyof Holiday}
            currentSortDescending={hookInstance.filter.sortDescending}
          />
        </>
      )}
    </div>
  );

  if (!mounted) {
    return null;
  }

  return (
    <div className="min-h-screen bg-gray-100">
      <header className="bg-white shadow">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 flex justify-between items-center">
          <h1 className="text-2xl font-bold text-gray-900">Brazilian Holidays 2025</h1>
          <button
            onClick={handleLogout}
            className="px-4 py-2 bg-red-600 text-white rounded-md hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-red-500"
          >
            Logout
          </button>
        </div>
      </header>

      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <Tabs
          tabs={[
            {
              id: 'all',
              label: 'All Holidays',
              content: renderHolidayTab(allHolidays),
            },
            {
              id: 'national',
              label: 'National Holidays',
              content: renderHolidayTab(nationalHolidays),
            },
            {
              id: 'municipal',
              label: 'Municipal Holidays',
              content: renderHolidayTab(municipalHolidays),
            },
          ]}
        />
      </main>
    </div>
  );
}

