'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { authService } from '@/lib/api';

const loginSchema = z.object({
  username: z.string().min(1, 'Usuário é obrigatório'),
  password: z.string().min(1, 'Senha é obrigatória'),
});

type LoginFormData = z.infer<typeof loginSchema>;

export default function LoginPage() {
  const router = useRouter();
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormData>({
    resolver: zodResolver(loginSchema),
  });

  const onSubmit = async (data: LoginFormData) => {
    setLoading(true);
    setError(null);

    try {
      const response = await authService.login(data);

      if (response.success) {
        router.push('/');
      } else {
        setError(response.message);
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Falha no login');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-white p-4">
      <div className="bg-white lg:bg-gray-50 p-6 md:p-8 rounded-lg w-full max-w-md">
        <h1 className="text-xl md:text-2xl font-semibold text-center mb-6 md:mb-8 text-gray-900">Feriados Brasileiros</h1>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-5 md:space-y-6">
          <div>
            <label htmlFor="username" className="block text-xs md:text-sm font-medium text-gray-700 mb-2">
              Usuario
            </label>
            <input
              {...register('username')}
              id="username"
              type="text"
              autoComplete="username"
              className="w-full px-0 py-2 md:py-3 border-0 border-b border-gray-300 bg-transparent focus:outline-none focus:ring-0 focus:border-blue-500 text-gray-900 text-sm md:text-base"
              placeholder="admin"
            />
            {errors.username && (
              <p className="mt-1 text-xs md:text-sm text-red-600">{errors.username.message}</p>
            )}
          </div>

          <div>
            <label htmlFor="password" className="block text-xs md:text-sm font-medium text-gray-700 mb-2">
              Senha
            </label>
            <input
              {...register('password')}
              id="password"
              type="password"
              autoComplete="current-password"
              className="w-full px-0 py-2 md:py-3 border-0 border-b border-gray-300 bg-transparent focus:outline-none focus:ring-0 focus:border-blue-500 text-gray-900 text-sm md:text-base"
              placeholder="admin"
            />
            {errors.password && (
              <p className="mt-1 text-xs md:text-sm text-red-600">{errors.password.message}</p>
            )}
          </div>

          {error && (
            <div className="p-3 bg-red-50 border border-red-200 rounded-md">
              <p className="text-xs md:text-sm text-red-600">{error}</p>
            </div>
          )}

          <button
            type="submit"
            disabled={loading}
            className="w-full py-2.5 md:py-3 px-4 bg-blue-800 text-white text-sm md:text-base rounded-full hover:bg-blue-900 focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:opacity-50 transition-colors"
          >
            {loading ? 'Entrando...' : 'Entrar'}
          </button>
        </form>

        <div className="mt-5 md:mt-6 p-3 bg-white rounded-md">
          <p className="text-xs text-gray-600 text-center">
            Credenciais padrão: admin / admin
          </p>
        </div>
      </div>
    </div>
  );
}

