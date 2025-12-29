const mockPost = jest.fn();
const mockGet = jest.fn();
const mockUse = jest.fn();

jest.mock('axios', () => ({
  create: jest.fn(() => ({
    post: mockPost,
    get: mockGet,
    interceptors: {
      response: { use: mockUse },
    },
  })),
}));

describe('api services', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    jest.resetModules();
  });

  it('realiza login e retorna dados', async () => {
    const { authService } = await import('@/lib/api');

    mockPost.mockResolvedValue({
      data: {
        success: true,
        data: { username: 'user', expiresAt: '2025-01-01' },
        message: '',
        totalRecords: 0,
      },
    });

    const credentials = { username: 'user', password: 'pass' };
    const result = await authService.login(credentials);

    expect(mockPost).toHaveBeenCalledWith('/auth/login', credentials);
    expect(result.success).toBe(true);
  });

  it('realiza logout chamando endpoint correto', async () => {
    const { authService } = await import('@/lib/api');

    mockPost.mockResolvedValue({});

    await authService.logout();

    expect(mockPost).toHaveBeenCalledWith('/auth/logout');
  });

  it('monta parametros corretamente em getHolidays', async () => {
    const { holidayService } = await import('@/lib/api');

    mockGet.mockResolvedValue({
      data: {
        success: true,
        data: [],
        message: '',
        totalRecords: 0,
      },
    });

    await holidayService.getHolidays({
      year: 2025,
      date: '2025-01-01',
      type: 'National',
      searchTerm: 'Ano',
      sortBy: 'date',
      sortDescending: true,
    });

    const calledUrl = mockGet.mock.calls[0][0] as string;
    expect(calledUrl).toContain('/holidays?');
    expect(calledUrl).toContain('year=2025');
    expect(calledUrl).toContain('date=2025-01-01');
    expect(calledUrl).toContain('type=National');
    expect(calledUrl).toContain('searchTerm=Ano');
    expect(calledUrl).toContain('sortBy=date');
    expect(calledUrl).toContain('sortDescending=true');
  });

  it('redireciona para login quando resposta 401', async () => {
    const originalLocation = window.location;

    Object.defineProperty(window, 'location', {
      value: { pathname: '/home', href: '' },
      writable: true,
    });

    await import('@/lib/api');

    const errorHandler = mockUse.mock.calls[0][1];

    const error = { response: { status: 401 } };

    await expect(errorHandler(error)).rejects.toBe(error);
    expect(window.location.href).toBe('/login');

    Object.defineProperty(window, 'location', {
      value: originalLocation,
      writable: true,
    });
  });

  it('redireciona para login quando nao ha response', async () => {
    const originalLocation = window.location;

    Object.defineProperty(window, 'location', {
      value: { pathname: '/home', href: '' },
      writable: true,
    });

    await import('@/lib/api');

    const errorHandler = mockUse.mock.calls[0][1];

    const error = {};

    await expect(errorHandler(error)).rejects.toBe(error);
    expect(window.location.href).toBe('/login');

    Object.defineProperty(window, 'location', {
      value: originalLocation,
      writable: true,
    });
  });
});
