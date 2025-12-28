import React, { useState } from 'react';
import type { HolidayFilter } from '@/types/holiday';

interface HolidayFiltersProps {
  onFilterChange: (filter: HolidayFilter) => void;
}

export function HolidayFilters({ onFilterChange }: HolidayFiltersProps) {
  const currentYear = new Date().getFullYear();
  const [selectedYear, setSelectedYear] = useState(currentYear);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedType, setSelectedType] = useState('');

  const handleSearch = () => {
    onFilterChange({
      year: selectedYear,
      searchTerm: searchTerm || undefined,
      type: selectedType || undefined,
    });
  };

  const handleClear = () => {
    setSelectedYear(currentYear);
    setSearchTerm('');
    setSelectedType('');
    onFilterChange({ year: currentYear });
  };

  return (
    <div className="bg-white p-4 rounded-lg shadow mb-6">
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Ano
          </label>
          <select
            value={selectedYear}
            onChange={(e) => setSelectedYear(Number(e.target.value))}
            className="w-full h-10 px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900"
          >
            {Array.from({ length: 10 }, (_, i) => currentYear - 5 + i).map((year) => (
              <option key={year} value={year}>
                {year}
              </option>
            ))}
          </select>
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Nome
          </label>
          <input
            type="text"
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            placeholder="Nome do feriado..."
            className="w-full h-10 px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900"
            onKeyDown={(e) => e.key === 'Enter' && handleSearch()}
          />
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Tipo
          </label>
          <select
            value={selectedType}
            onChange={(e) => setSelectedType(e.target.value)}
            className="w-full h-10 px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900"
          >
            <option value="">Todos</option>
            <option value="National">Nacional</option>
            <option value="Municipal">Municipal</option>
          </select>
        </div>
      </div>

      <div className="flex gap-2 mt-4 justify-end">
        <button
          onClick={handleClear}
          className="px-4 py-2 bg-gray-200 text-gray-700 rounded-md hover:bg-gray-300 focus:outline-none focus:ring-2 focus:ring-gray-500 flex items-center gap-2"
        >
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" className="w-5 h-5">
            <path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
          </svg>
          Limpar
        </button>
        <button
          onClick={handleSearch}
          className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 flex items-center gap-2"
        >
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" className="w-5 h-5">
            <path strokeLinecap="round" strokeLinejoin="round" d="M21 21l-5.197-5.197m0 0A7.5 7.5 0 105.196 5.196a7.5 7.5 0 0010.607 10.607z" />
          </svg>
          Filtrar
        </button>
      </div>
    </div>
  );
}

