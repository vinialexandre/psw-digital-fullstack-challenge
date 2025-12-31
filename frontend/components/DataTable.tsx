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

  return (
    <div className="space-y-0 overflow-x-auto">
      <div className="min-w-[600px] md:min-w-0">
        <div className="flex items-center px-0 py-3 border-b border-gray-200">
          <div className="w-48 md:w-80 text-xs md:text-sm font-semibold text-gray-700 cursor-pointer hover:text-gray-900" onClick={() => handleSort(columns[0])}>
            {columns[0].header}
          </div>
          <div className="w-28 md:w-32 text-xs md:text-sm font-semibold text-gray-700 cursor-pointer hover:text-gray-900 ml-2 md:ml-3" onClick={() => handleSort(columns[1])}>
            {columns[1].header}
          </div>
          <div className="w-28 md:w-32 text-xs md:text-sm font-semibold text-gray-700 cursor-pointer hover:text-gray-900 ml-12 md:ml-24" onClick={() => handleSort(columns[2])}>
            {columns[2].header}
          </div>
        </div>

        {data.map((item, index) => (
          <div
            key={index}
            onClick={() => onRowClick?.(item)}
            className="flex items-center justify-between px-0 py-4 md:py-6 border-b border-gray-100 hover:bg-gray-50 cursor-pointer"
          >
            <div className="flex items-center">
              <div className="w-48 md:w-80 text-xs md:text-sm text-gray-900 whitespace-nowrap overflow-hidden text-ellipsis">
                {String(item[columns[0].key])}
              </div>
              <div className="w-28 md:w-32 text-xs md:text-sm text-gray-900 whitespace-nowrap overflow-hidden text-ellipsis ml-2 md:ml-3">
                {String(item[columns[1].key])}
              </div>
              <div className="w-28 md:w-32 text-xs md:text-sm text-gray-900 ml-12 md:ml-24">
                {columns[2].render
                  ? columns[2].render(item[columns[2].key], item)
                  : String(item[columns[2].key])}
              </div>
            </div>
            <button className="text-gray-400 hover:text-gray-600 flex-shrink-0 ml-2">
              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" className="w-4 h-4 md:w-5 md:h-5">
                <path strokeLinecap="round" strokeLinejoin="round" d="M9 5l7 7-7 7" />
              </svg>
            </button>
          </div>
        ))}
      </div>
    </div>
  );
}
