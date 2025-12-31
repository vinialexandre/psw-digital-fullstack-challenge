import React, { useState } from 'react';
import type { HolidayFilter } from '@/types/holiday';

interface HolidayFiltersProps {
  readonly onFilterChange: (filter: HolidayFilter) => void;
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

  return (
    <div className="space-y-4">
      <input
        type="text"
        value={searchTerm}
        onChange={(e) => setSearchTerm(e.target.value)}
        placeholder="Busque por nome"
        className="w-full px-0 py-2 border-0 border-b border-gray-300 text-xs md:text-sm focus:outline-none focus:ring-0 focus:border-blue-500 text-gray-900 placeholder-gray-400 bg-transparent"
        onKeyDown={(e) => e.key === 'Enter' && handleSearch()}
      />

      <div className="flex flex-col sm:flex-row gap-3">
        <div className="flex-1">
          <label htmlFor="type-filter" className="block text-xs text-gray-500 mb-1">Tipo</label>
          <select
            id="type-filter"
            value={selectedType}
            onChange={(e) => setSelectedType(e.target.value)}
            className="w-full px-3 py-2 border border-gray-300 rounded-full text-xs md:text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900 bg-white"
          >
            <option value="">Todos</option>
            <option value="National">Nacional</option>
            <option value="Municipal">Municipal</option>
          </select>
        </div>

        <div className="flex-1">
          <label htmlFor="year-filter" className="block text-xs text-gray-500 mb-1">Data do Feriado</label>
          <select
            id="year-filter"
            value={selectedYear}
            onChange={(e) => setSelectedYear(Number(e.target.value))}
            className="w-full px-3 py-2 border border-gray-300 rounded-full text-xs md:text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900 bg-white"
          >
            {Array.from({ length: 10 }, (_, i) => currentYear - 5 + i).map((year) => (
              <option key={year} value={year}>
                {year}
              </option>
            ))}
          </select>
        </div>
      </div>
    </div>
  );
}
