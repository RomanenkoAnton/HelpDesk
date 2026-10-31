import type { ApiResponse } from '@/types/api'

export async function request<T>(url: string, options?: RequestInit): Promise<T> {
  const response = await fetch(url, options)

  let data: ApiResponse<T> | null = null
  try {
    data = await response.json()
  } catch {
    // у ответа нет тела
  }

  if (!response.ok || !data?.isSuccess) {
    throw new Error(data?.errorMessages?.join(', ') || `Ошибка ${response.status}`)
  }
  return data.result
}

export function sendJson<T>(url: string, method: 'POST' | 'PUT', body: unknown) {
  return request<T>(url, {
    method,
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  })
}
