import React from 'react';

export interface Column<T> {
  key: keyof T;
  header: string;
  sortable?: boolean;
  render?: (value: T[keyof T], item: T) => React.ReactNode;
}

interface DataTableProps<T> {
  data: T[];
  columns: Column<T>[];
  onSort?: (field: keyof T, descending: boolean) => void;
  currentSortField?: keyof T;
  currentSortDescending?: boolean;
  onRowClick?: (item: T) => void;
}

export function DataTable<T extends Record<string, unknown>>({
  data,
  columns,
  onSort,
  currentSortField,
  currentSortDescending = false,
  onRowClick,
}: DataTableProps<T>) {
  const handleSort = (column: Column<T>) => {
    if (column.sortable && onSort) {
      const isCurrentField = currentSortField === column.key;
      const newDescending = isCurrentField ? !currentSortDescending : false;
      onSort(column.key, newDescending);
    }
  };

  const getColumnWidth = (key: keyof T): string => {
    if (key === 'name') return 'w-48 md:w-80';
    if (key === 'date') return 'w-28 md:w-32 ml-2 md:ml-3';
    return 'w-28 md:w-32 ml-12 md:ml-24';
  };

  const getCellWidth = (key: keyof T): string => {
    if (key === 'name') return 'w-48 md:w-80 whitespace-nowrap overflow-hidden text-ellipsis';
    if (key === 'date') return 'w-28 md:w-32 whitespace-nowrap overflow-hidden text-ellipsis ml-2 md:ml-3';
    return 'w-28 md:w-32 ml-12 md:ml-24';
  };

  const getItemName = (item: T): string => {
    const name = item.name;
    if (typeof name === 'string') return name;
    if (name && typeof name === 'object') return JSON.stringify(name);
    return name ? String(name) : '';
  };

  return (
    <div className="space-y-0 overflow-x-auto">
      <div className="min-w-[600px] md:min-w-0">
        <div className="flex items-center px-0 py-3 border-b border-gray-200">
          {columns.map((column) => (
            <button
              key={String(column.key)}
              className={`text-xs md:text-sm font-semibold text-gray-700 hover:text-gray-900 text-left ${getColumnWidth(column.key)}`}
              onClick={() => handleSort(column)}
              disabled={!column.sortable}
              aria-label={`Ordenar por ${column.header}`}
            >
              {column.header}
            </button>
          ))}
        </div>

        {data.map((item, index) => {
          const itemId = (item.id as string | number) || (item.date as string) || index;
          return (
            <button
              key={String(itemId)}
              onClick={() => onRowClick?.(item)}
              className="flex items-center justify-between px-0 py-4 md:py-6 border-b border-gray-100 hover:bg-gray-50 w-full text-left"
              aria-label={`Ver detalhes do feriado ${getItemName(item)}`}
            >
              <div className="flex items-center">
                {columns.map((column) => (
                  <div
                    key={String(column.key)}
                    className={`text-xs md:text-sm text-gray-900 ${getCellWidth(column.key)}`}
                  >
                    {column.render
                      ? column.render(item[column.key], item)
                      : String(item[column.key])}
                  </div>
                ))}
              </div>
              <span className="text-gray-400 hover:text-gray-600 flex-shrink-0 ml-2" aria-hidden="true">
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" className="w-4 h-4 md:w-5 md:h-5">
                  <path strokeLinecap="round" strokeLinejoin="round" d="M9 5l7 7-7 7" />
                </svg>
              </span>
            </button>
          );
        })}
      </div>
    </div>
  );
}
