<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { Pencil, Trash2, Plus, Search, Check } from 'lucide-vue-next'
import { request, sendJson } from '@/api/request'
import type { Ticket, Department } from '@/types/api'

const tickets = ref<Ticket[]>([])
const departments = ref<Department[]>([])

const loading = ref(true)
const error = ref('')
const search = ref('')
const filterDepartment = ref(0)

// форма
const showForm = ref(false)
const editingId = ref<number | null>(null)
const saving = ref(false)
const formError = ref('')
const form = reactive({
  model: '',
  serialNumber: '',
  departmentId: 0,
  notes: '',
  setupDate: '',
  sentToSZIDate: '',
  returnedFromSZIDate: '',
  issuedDate: '',
  assignedEmployee: '',
  issuedTo: '',
  invoiceSigned: false,
})

// "2026-10-31T00:00:00" -> "2026-10-31" (для input type="date")
function toInput(value: string | null): string {
  return value ? value.slice(0, 10) : ''
}

// "2026-10-31" -> "2026-10-31T00:00:00" или null
function toApi(value: string): string | null {
  return value ? `${value}T00:00:00` : null
}

// "2026-10-31T..." -> "31.10.2026"
function formatDate(value: string | null): string {
  if (!value) return ''
  const [y, m, d] = value.slice(0, 10).split('-')
  return `${d}.${m}.${y}`
}

function getStatus(t: Ticket): { text: string; cls: string } {
  if (t.issuedDate) return { text: 'Выдано', cls: 'done' }
  if (t.returnedFromSZIDate) return { text: 'Повернуто з СЗІ (дата)', cls: 'back' }
  if (t.sentToSZIDate) return { text: 'Передано в СЗІ (дата)', cls: 'szi' }
  if (t.setupDate) return { text: 'Налаштовано', cls: 'setup' }
  return { text: 'Новая', cls: 'new' }
}

const filtered = computed(() => {
  const q = search.value.trim().toLowerCase()
  return tickets.value.filter((t) => {
    if (filterDepartment.value && t.departmentId !== filterDepartment.value) return false
    if (!q) return true
    return [t.model, t.serialNumber, t.departmentTitle, t.assignedEmployee, t.issuedTo, t.notes]
      .filter(Boolean)
      .some((v) => v!.toLowerCase().includes(q))
  })
})

onMounted(async () => {
  try {
    const [t, d] = await Promise.all([
      request<Ticket[]>('/api/tickets'),
      request<Department[]>('/api/departments'),
    ])
    tickets.value = t
    departments.value = d
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'Не удалось загрузить данные'
  } finally {
    loading.value = false
  }
})

function openCreate() {
  editingId.value = null
  form.model = ''
  form.serialNumber = ''
  form.departmentId = departments.value[0]?.id ?? 0
  form.notes = ''
  form.setupDate = ''
  form.sentToSZIDate = ''
  form.returnedFromSZIDate = ''
  form.issuedDate = ''
  form.assignedEmployee = ''
  form.issuedTo = ''
  form.invoiceSigned = false
  formError.value = ''
  showForm.value = true
}

function openEdit(t: Ticket) {
  editingId.value = t.id
  form.model = t.model
  form.serialNumber = t.serialNumber
  form.departmentId = t.departmentId
  form.notes = t.notes ?? ''
  form.setupDate = toInput(t.setupDate)
  form.sentToSZIDate = toInput(t.sentToSZIDate)
  form.returnedFromSZIDate = toInput(t.returnedFromSZIDate)
  form.issuedDate = toInput(t.issuedDate)
  form.assignedEmployee = t.assignedEmployee ?? ''
  form.issuedTo = t.issuedTo ?? ''
  form.invoiceSigned = t.invoiceSigned
  formError.value = ''
  showForm.value = true
}

function closeForm() {
  if (saving.value) return
  showForm.value = false
}

function buildPayload() {
  return {
    model: form.model.trim(),
    serialNumber: form.serialNumber.trim(),
    departmentId: form.departmentId,
    notes: form.notes.trim() || null,
    setupDate: toApi(form.setupDate),
    sentToSZIDate: toApi(form.sentToSZIDate),
    returnedFromSZIDate: toApi(form.returnedFromSZIDate),
    issuedDate: toApi(form.issuedDate),
    assignedEmployee: form.assignedEmployee.trim() || null,
    issuedTo: form.issuedTo.trim() || null,
    invoiceSigned: form.invoiceSigned,
  }
}

async function save() {
  if (!form.model.trim() || !form.serialNumber.trim()) {
    formError.value = 'Заполните модель и серийный номер'
    return
  }
  if (!form.departmentId) {
    formError.value = 'Выберите подразделение'
    return
  }

  saving.value = true
  formError.value = ''
  try {
    if (editingId.value === null) {
      const created = await sendJson<Ticket>('/api/ticket', 'POST', buildPayload())
      tickets.value.unshift(created)
    } else {
      const updated = await sendJson<Ticket>('/api/ticket', 'PUT', {
        id: editingId.value,
        ...buildPayload(),
      })
      const index = tickets.value.findIndex((x) => x.id === editingId.value)
      if (index !== -1) tickets.value[index] = updated
    }
    showForm.value = false
  } catch (e) {
    formError.value = e instanceof Error ? e.message : 'Не удалось сохранить'
  } finally {
    saving.value = false
  }
}

async function remove(t: Ticket) {
  if (!confirm(`Удалить заявку «${t.model}» (${t.serialNumber})?`)) return
  try {
    await request<unknown>(`/api/ticket/${t.id}`, { method: 'DELETE' })
    tickets.value = tickets.value.filter((x) => x.id !== t.id)
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'Не удалось удалить'
  }
}
</script>

<template>
  <section class="page">
    <div class="toolbar">
      <button class="btn btn-primary" :disabled="loading || !!error" @click="openCreate">
        <Plus :size="16" /> Додати
      </button>

      <select v-model.number="filterDepartment" class="field filter">
        <option :value="0">Всі підрозділи</option>
        <option v-for="d in departments" :key="d.id" :value="d.id">{{ d.title }}</option>
      </select>

      <label class="search">
        <Search :size="16" />
        <input v-model="search" class="field" placeholder="Поиск..." />
      </label>
    </div>

    <div class="table-area">
      <p v-if="loading" class="info">Загрузка...</p>
      <p v-else-if="error" class="info error">{{ error }}</p>
      <p v-else-if="tickets.length === 0" class="info">Заявок пока нет. Нажмите «Добавить».</p>

      <table v-else>
        <thead>
          <tr>
            <th class="num">№</th>
            <th>Модель</th>
            <th>Серійный номер</th>
            <th>На яку службу видається</th>
            <th>Статус</th>
            <th>Налаштування</th>
            <th>Кому видано</th>
            <th>Накладна</th>
            <th>Примітки</th>
            <th class="actions-col"></th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="t in filtered" :key="t.id">
            <td class="num">{{ t.id }}</td>
            <td>{{ t.model }}</td>
            <td>{{ t.serialNumber }}</td>
            <td>{{ t.departmentTitle }}</td>
            <td>
              <span class="badge" :class="getStatus(t).cls">{{ getStatus(t).text }}</span>
            </td>
            <td>{{ formatDate(t.setupDate) }}</td>
            <td>{{ t.issuedTo }}</td>
            <td class="center">
              <Check v-if="t.invoiceSigned" :size="16" class="check" />
            </td>
            <td>{{ t.notes }}</td>
            <td class="actions">
              <button class="icon-btn" title="Редагувати" @click="openEdit(t)">
                <Pencil :size="16" />
              </button>
              <button class="icon-btn danger" title="Видалити" @click="remove(t)">
                <Trash2 :size="16" />
              </button>
            </td>
          </tr>
          <tr v-if="filtered.length === 0">
            <td colspan="9" class="empty">Не знайдено</td>
          </tr>
        </tbody>
      </table>
    </div>

    <footer class="bottom-bar">
      <span class="hint">Показано: {{ filtered.length }} из {{ tickets.length }}</span>
    </footer>

    <!-- форма -->
    <div v-if="showForm" class="overlay" @click.self="closeForm">
      <form class="modal" @submit.prevent="save">
        <h2>{{ editingId === null ? 'Нова заявка' : `Заявка № ${editingId}` }}</h2>

        <div class="grid">
          <label>
            Модель
            <input v-model="form.model" class="field" :disabled="saving" autofocus />
          </label>

          <label>
            Серійный номер
            <input v-model="form.serialNumber" class="field" :disabled="saving" />
          </label>

          <label class="wide">
            На яку службу видається
            <select v-model.number="form.departmentId" class="field" :disabled="saving">
              <option v-for="d in departments" :key="d.id" :value="d.id">{{ d.title }}</option>
            </select>
          </label>

          <label>
            Дата налаштування
            <input v-model="form.setupDate" type="date" class="field" :disabled="saving" />
          </label>

          <label>
            Дата видачи
            <input v-model="form.issuedDate" type="date" class="field" :disabled="saving" />
          </label>

          <label>
            Відправлено в СЗИ
            <input v-model="form.sentToSZIDate" type="date" class="field" :disabled="saving" />
          </label>

          <label>
            Повернулось из СЗИ
            <input v-model="form.returnedFromSZIDate" type="date" class="field" :disabled="saving" />
          </label>

          <label>
            Виконавець
            <input v-model="form.assignedEmployee" class="field" :disabled="saving" />
          </label>

          <label>
            Кому видано
            <input v-model="form.issuedTo" class="field" :disabled="saving" />
          </label>

          <label class="wide">
            Примітки
            <textarea v-model="form.notes" class="field" rows="3" :disabled="saving"></textarea>
          </label>

          <label class="wide checkbox">
            <input v-model="form.invoiceSigned" type="checkbox" :disabled="saving" />
            Накладна підписана
          </label>
        </div>

        <p v-if="formError" class="error">{{ formError }}</p>

        <div class="modal-actions">
          <button type="button" class="btn" :disabled="saving" @click="closeForm">Відміна</button>
          <button type="submit" class="btn btn-primary" :disabled="saving">
            {{ saving ? 'Збереження...' : 'Зберегти' }}
          </button>
        </div>
      </form>
    </div>
  </section>
</template>

<style scoped>
.page {
  height: 100%;
  display: flex;
  flex-direction: column;
}

.toolbar {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 10px 16px;
  border-bottom: 1px solid var(--border);
}
.filter {
  width: 220px;
}
.search {
  display: flex;
  align-items: center;
  gap: 6px;
  margin-left: auto;
  width: 260px;
  color: #666;
}

.table-area {
  flex: 1;
  overflow: auto;
  padding: 12px 16px;
}
.info {
  padding: 4px 0;
}
.error {
  color: #c00;
}

table {
  width: 100%;
  border-collapse: collapse;
  font-size: 13px;
}
th {
  position: sticky;
  top: 0;
  background: var(--header-bg);
  text-align: left;
  font-weight: 600;
  padding: 4px 10px;
  border: 1px solid var(--border);
  white-space: nowrap;
}
td {
  height: 26px;
  padding: 2px 10px;
  border: 1px solid var(--border);
  background: var(--green-row);
  line-height: 1.2;
}
tbody tr:nth-child(even) td {
  background: var(--green-row-alt);
}
tbody tr:hover td:not(.num) {
  background: #e8fbcf;
}
td.num,
th.num {
  width: 50px;
  text-align: right;
  background: #fff;
  color: #555;
}
td.center {
  text-align: center;
}
td.empty {
  background: #fff;
  text-align: center;
  color: #777;
}
.check {
  color: #2e7d32;
  vertical-align: middle;
}

/* статусы */
.badge {
  display: inline-block;
  padding: 1px 8px;
  border-radius: 10px;
  font-size: 12px;
  white-space: nowrap;
  border: 1px solid transparent;
}
.badge.new {
  background: #fff;
  border-color: #bbb;
  color: #555;
}
.badge.setup {
  background: #e3f2fd;
  border-color: #90caf9;
  color: #1565c0;
}
.badge.szi {
  background: var(--yellow-soft);
  border-color: var(--yellow);
  color: #6b5800;
}
.badge.back {
  background: #ffe0b2;
  border-color: #ffb74d;
  color: #8a4b00;
}
.badge.done {
  background: #2e7d32;
  color: #fff;
}

.actions-col {
  width: 72px;
}
.actions {
  white-space: nowrap;
  text-align: right;
  padding: 0 6px;
}
.icon-btn {
  background: none;
  border: none;
  padding: 3px;
  border-radius: 4px;
  cursor: pointer;
  color: #4b5a63;
  display: inline-flex;
  align-items: center;
  vertical-align: middle;
}
.icon-btn:hover {
  background: rgba(0, 0, 0, 0.08);
  color: #111;
}
.icon-btn.danger:hover {
  color: #c62828;
}

.bottom-bar {
  display: flex;
  align-items: center;
  padding: 8px 16px;
  background: #fff;
  border-top: 1px solid var(--border);
}
.hint {
  color: #777;
  font-size: 12px;
}

/* форма */
.overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.4);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 100;
}
.modal {
  width: 600px;
  max-width: calc(100vw - 32px);
  max-height: calc(100vh - 32px);
  overflow: auto;
  background: #fff;
  border: 1px solid var(--border);
  border-top: 4px solid var(--yellow);
  border-radius: 4px;
  padding: 16px 20px 20px;
  display: flex;
  flex-direction: column;
  gap: 12px;
}
.modal h2 {
  margin: 0;
  font-size: 16px;
}
.grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 12px;
}
.grid label {
  display: flex;
  flex-direction: column;
  gap: 4px;
  font-size: 13px;
  color: #444;
}
.grid .wide {
  grid-column: 1 / -1;
}
.grid .checkbox {
  flex-direction: row;
  align-items: center;
  gap: 8px;
}
.modal .field {
  padding: 6px 8px;
  font-family: inherit;
}
.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 8px;
}
</style>
