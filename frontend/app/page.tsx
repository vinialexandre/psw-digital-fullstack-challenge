'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { DataTable, Column } from '@/components/DataTable';
import { useHolidays } from '@/hooks/useHolidays';
import { authService } from '@/lib/api';
import type { Holiday } from '@/types/holiday';

export default function HomePage() {
  const router = useRouter();
  const [mounted, setMounted] = useState(false);
  const currentYear = new Date().getFullYear();
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedYear, setSelectedYear] = useState<number | string>('');
  const [selectedType, setSelectedType] = useState('');
  const [sortField, setSortField] = useState<string>('');
  const [selectedHoliday, setSelectedHoliday] = useState<Holiday | null>(null);

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

  const handleSortChange = (field: string) => {
    if (!field) return;
    const isCurrentField = allHolidays.filter.sortBy === field;
    const newDescending = isCurrentField ? !allHolidays.filter.sortDescending : false;
    setSortField(field);
    handleSort(field as keyof Holiday, newDescending);
  };

  const handleSearch = () => {
    allHolidays.setFilter({
      ...allHolidays.filter,
      year: selectedYear ? Number(selectedYear) : currentYear,
      searchTerm: searchTerm || undefined,
      type: selectedType || undefined,
    });
  };

  const handleClearSearch = () => {
    setSearchTerm('');
    allHolidays.setFilter({
      ...allHolidays.filter,
      year: selectedYear ? Number(selectedYear) : currentYear,
      searchTerm: undefined,
      type: selectedType || undefined,
    });
  };

  const columns: Column<Holiday>[] = [
    {
      key: 'name',
      header: 'Nome',
      sortable: true,
    },
    {
      key: 'date',
      header: 'Data',
      sortable: true,
    },
    {
      key: 'type',
      header: 'Tipo',
      sortable: true,
      render: (value) => (
        <span
          className={`px-3 py-1 rounded-full text-xs font-semibold whitespace-nowrap text-gray-900`}
          style={{
            backgroundColor: value === 'National' ? '#b3e01e' : '#fdf10a'
          }}
        >
          {value === 'National' ? 'Nacional' : 'Municipal'}
        </span>
      ),
    },
  ];

  if (!mounted) {
    return null;
  }

  return (
    <div className="min-h-screen bg-white">
      <button
        onClick={handleLogout}
        className="fixed top-4 right-4 md:top-6 md:right-6 p-2 bg-white rounded-full shadow-sm text-gray-400 hover:text-gray-600 focus:outline-none z-50"
        aria-label="Sair"
      >
        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" className="w-5 h-5 md:w-6 md:h-6">
          <path strokeLinecap="round" strokeLinejoin="round" d="M15.75 9V5.25A2.25 2.25 0 0013.5 3h-6a2.25 2.25 0 00-2.25 2.25v13.5A2.25 2.25 0 007.5 21h6a2.25 2.25 0 002.25-2.25V15m3 0l3-3m0 0l-3-3m3 3H9" />
        </svg>
      </button>

      <div className="px-4 md:px-8 lg:px-12 pt-8 md:pt-12 pb-6">
        <h2 className="text-xs md:text-sm text-gray-400 mb-6 md:mb-8">MEUS FERIADOS</h2>

        <nav className="flex gap-4 md:gap-8 lg:gap-20 mb-6 md:mb-8 border-b border-gray-200 md:pl-12 overflow-x-auto scrollbar-hide" role="navigation" aria-label="Menu de navegação">
          <button className="text-sm md:text-base text-gray-400 hover:text-gray-600 pb-3 whitespace-nowrap">Tela A</button>
          <button className="text-sm md:text-base text-gray-400 hover:text-gray-600 pb-3 whitespace-nowrap">Tela B</button>
          <button className="text-sm md:text-base text-gray-400 hover:text-gray-600 pb-3 whitespace-nowrap">Tela C</button>
          <button className="text-sm md:text-base font-semibold text-gray-900 pb-3 border-b-4 border-blue-800 -mb-px px-4 md:px-8 whitespace-nowrap" aria-current="page">Feriados</button>
        </nav>

        <div className="mb-6">
          <div className="flex items-center gap-2 md:gap-4">
            <div className="flex-1 relative max-w-full md:max-w-[900px]">
              <input
                type="text"
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                placeholder="Busque por nome"
                className="w-full px-0 py-3 md:py-4 border-0 border-b border-gray-300 text-sm focus:outline-none focus:ring-0 focus:border-blue-500 text-gray-900 placeholder-gray-400 bg-transparent"
                onKeyDown={(e) => e.key === 'Enter' && handleSearch()}
              />
              {searchTerm && (
                <button
                  onClick={handleClearSearch}
                  className="absolute right-0 top-1/2 -translate-y-1/2 text-gray-400 hover:text-red-500 transition-colors"
                >
                  <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" className="w-4 h-4 md:w-5 md:h-5">
                    <path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
                  </svg>
                </button>
              )}
            </div>
            <button onClick={handleSearch} className="w-8 h-8 md:w-9 md:h-9 rounded-full bg-blue-800 text-white flex items-center justify-center hover:bg-blue-900 flex-shrink-0" aria-label="Buscar feriados">
              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2.5} stroke="currentColor" className="w-3.5 h-3.5 md:w-4 md:h-4">
                <path strokeLinecap="round" strokeLinejoin="round" d="M21 21l-5.197-5.197m0 0A7.5 7.5 0 105.196 5.196a7.5 7.5 0 0010.607 10.607z" />
              </svg>
            </button>
            <div className="hidden md:flex items-center ml-6">
              <span className="text-sm text-gray-500 flex-shrink-0 mr-12">{allHolidays.totalRecords} REGISTROS</span>
              <div className="relative">
                <label htmlFor="sort-select-desktop" className="sr-only">Ordenar por</label>
                <select
                  id="sort-select-desktop"
                  value={sortField}
                  onChange={(e) => handleSortChange(e.target.value)}
                  className="text-sm font-semibold text-gray-500 pr-6 bg-transparent border-0 focus:outline-none cursor-pointer appearance-none uppercase text-right"
                >
                  <option value="">ORDENAR POR</option>
                  <option value="name">NOME</option>
                  <option value="date">DATA</option>
                  <option value="type">TIPO</option>
                </select>
                <svg className="absolute right-0 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-700 pointer-events-none" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" d="M19.5 8.25l-7.5 7.5-7.5-7.5" />
                </svg>
              </div>
            </div>
          </div>

          <div className="flex md:hidden items-center justify-between mt-4">
            <span className="text-xs text-gray-500">{allHolidays.totalRecords} REGISTROS</span>
            <div className="relative">
              <label htmlFor="sort-select-mobile" className="sr-only">Ordenar por</label>
              <select
                id="sort-select-mobile"
                value={sortField}
                onChange={(e) => handleSortChange(e.target.value)}
                className="text-xs font-semibold text-gray-500 pr-6 bg-transparent border-0 focus:outline-none cursor-pointer appearance-none uppercase text-right"
              >
                <option value="">ORDENAR POR</option>
                <option value="name">NOME</option>
                <option value="date">DATA</option>
                <option value="type">TIPO</option>
              </select>
              <svg className="absolute right-0 top-1/2 -translate-y-1/2 w-3 h-3 text-gray-700 pointer-events-none" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" d="M19.5 8.25l-7.5 7.5-7.5-7.5" />
              </svg>
            </div>
          </div>
        </div>

        <div className="mb-6 flex gap-3 md:gap-4 w-full max-w-full md:w-auto" style={{ maxWidth: '100%' }}>
          <div className="relative flex-1 md:flex-none" style={{ maxWidth: '160px' }}>
            <label htmlFor="type-filter" className="sr-only">Filtrar por tipo</label>
            <select
              id="type-filter"
              value={selectedType}
              onChange={(e) => setSelectedType(e.target.value)}
              className="w-full py-2 pr-10 md:pr-16 border-2 border-gray-400 rounded-full text-xs md:text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900 bg-white appearance-none text-center md:text-right"
            >
              <option value="">Tipo</option>
              <option value="National">Nacional</option>
              <option value="Municipal">Municipal</option>
            </select>
            <svg className="absolute right-3 md:right-4 top-1/2 -translate-y-1/2 w-3 h-3 md:w-4 md:h-4 text-gray-500 pointer-events-none" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" d="M19.5 8.25l-7.5 7.5-7.5-7.5" />
            </svg>
          </div>

          <div className="relative flex-1 md:flex-none" style={{ maxWidth: '200px' }}>
            <label htmlFor="year-filter" className="sr-only">Filtrar por ano</label>
            <select
              id="year-filter"
              value={selectedYear}
              onChange={(e) => setSelectedYear(e.target.value)}
              className="w-full pl-3 md:pl-4 py-2 pr-10 md:pr-14 border-2 border-gray-400 rounded-full text-xs md:text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900 bg-white appearance-none text-right"
            >
              <option value="">Data do Feriado</option>
              {Array.from({ length: 10 }, (_, i) => currentYear - 5 + i).map((year) => (
                <option key={year} value={year}>
                  {year}
                </option>
              ))}
            </select>
            <svg className="absolute right-3 md:right-4 top-1/2 -translate-y-1/2 w-3 h-3 md:w-4 md:h-4 text-gray-500 pointer-events-none" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" d="M19.5 8.25l-7.5 7.5-7.5-7.5" />
            </svg>
          </div>
        </div>

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
          <DataTable
            data={allHolidays.holidays}
            columns={columns}
            onSort={handleSort}
            currentSortField={allHolidays.filter.sortBy as keyof Holiday}
            currentSortDescending={allHolidays.filter.sortDescending}
            onRowClick={(holiday) => setSelectedHoliday(holiday)}
          />
        )}
      </div>

      {selectedHoliday && (
        <div
          className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4"
          onClick={() => setSelectedHoliday(null)}
        >
          <div
            className="bg-white rounded-lg p-6 md:p-8 max-w-md w-full"
            onClick={(e) => e.stopPropagation()}
          >
            <div className="flex justify-between items-start mb-4 md:mb-6">
              <h3 className="text-lg md:text-xl font-semibold text-gray-900">Detalhes do Feriado</h3>
              <button
                onClick={() => setSelectedHoliday(null)}
                className="text-gray-400 hover:text-gray-600 -mt-1 -mr-1"
              >
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" className="w-5 h-5 md:w-6 md:h-6">
                  <path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
                </svg>
              </button>
            </div>

            <div className="space-y-3 md:space-y-4">
              <div>
                <p className="text-xs md:text-sm font-semibold text-gray-500 uppercase">Nome</p>
                <p className="text-sm md:text-base text-gray-900 mt-1">{selectedHoliday.name}</p>
              </div>

              <div>
                <p className="text-xs md:text-sm font-semibold text-gray-500 uppercase">Data</p>
                <p className="text-sm md:text-base text-gray-900 mt-1">{selectedHoliday.date}</p>
              </div>

              <div>
                <p className="text-xs md:text-sm font-semibold text-gray-500 uppercase">Tipo</p>
                <div className="mt-1">
                  <span
                    className="inline-block px-3 py-1 rounded-full text-xs font-semibold text-gray-900"
                    style={{
                      backgroundColor: selectedHoliday.type === 'National' ? '#b3e01e' : '#fdf10a'
                    }}
                  >
                    {selectedHoliday.type === 'National' ? 'Nacional' : 'Municipal'}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
