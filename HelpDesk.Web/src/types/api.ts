export interface ApiResponse<T> {
  isSuccess: boolean
  statusCode: number
  errorMessages?: string[]
  result: T
}

export interface Department {
  id: number
  title: string
}

export interface Category {
  id: number
  title: string
}

export interface Unit {
  id: number
  title: string
  serialNumber: string
  categoryId: number
  categoryTitle: string
  departmentId: number
  departmentTitle: string
}


export interface Ticket {
  id: number
  model: string
  serialNumber: string
  departmentId: number
  departmentTitle: string
  notes: string | null
  setupDate: string | null
  sentToSZIDate: string | null
  returnedFromSZIDate: string | null
  issuedDate: string | null
  assignedEmployee: string | null
  issuedTo: string | null
  invoiceSigned: boolean
}
