import { renderHook, waitFor } from '@testing-library/react';
import { useHolidays } from '@/hooks/useHolidays';
import { holidayService } from '@/lib/api';

jest.mock('@/lib/api', () => ({
  holidayService: {
    getHolidays: jest.fn(),
  },
}));

const mockGetHolidays = holidayService.getHolidays as jest.Mock;

describe('useHolidays', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  it('carrega feriados com sucesso', async () => {
    mockGetHolidays.mockResolvedValue({
      success: true,
      data: [{ date: '2025-01-01', name: 'Ano Novo', type: 'National' }],
      message: '',
      totalRecords: 1,
    });

    const { result } = renderHook(() => useHolidays({ year: 2025 }));

    expect(result.current.loading).toBe(true);

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(mockGetHolidays).toHaveBeenCalledWith({ year: 2025 });
    expect(result.current.holidays).toHaveLength(1);
    expect(result.current.totalRecords).toBe(1);
    expect(result.current.error).toBeNull();
  });

  it('define mensagem de erro quando API retorna falha', async () => {
    mockGetHolidays.mockResolvedValue({
      success: false,
      data: [],
      message: 'Falha ao buscar',
      totalRecords: 0,
    });

    const { result } = renderHook(() => useHolidays());

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(result.current.error).toBe('Falha ao buscar');
  });

  it('nao define erro quando status 401', async () => {
    mockGetHolidays.mockRejectedValue({
      response: { status: 401 },
    });

    const { result } = renderHook(() => useHolidays());

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(result.current.error).toBeNull();
  });

  it('define erro generico quando resposta possui status diferente de 401', async () => {
    mockGetHolidays.mockRejectedValue({
      response: { status: 500 },
    });

    const { result } = renderHook(() => useHolidays());

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(result.current.error).toBe('Erro ao carregar feriados');
  });

  it('nao altera erro quando excecao nao possui response', async () => {
    mockGetHolidays.mockRejectedValue({});

    const { result } = renderHook(() => useHolidays());

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(result.current.error).toBeNull();
  });

  it('permite refazer busca ao alterar filtro', async () => {
    mockGetHolidays.mockResolvedValue({
      success: true,
      data: [],
      message: '',
      totalRecords: 0,
    });

    const { result, rerender } = renderHook(
      (props) => useHolidays(props),
      { initialProps: { year: 2024 } }
    );

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    result.current.setFilter({ year: 2025 });
    rerender({ year: 2025 });

    await waitFor(() => {
      expect(mockGetHolidays).toHaveBeenCalledTimes(2);
    });
  });
});
