<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { Pencil, Trash2, Plus, Search } from 'lucide-vue-next'
import { request, sendJson } from '@/api/request'
import type { Unit, Department, Category } from '@/types/api'

const units = ref<Unit[]>([])
const departments = ref<Department[]>([])
const categories = ref<Category[]>([])

const loading = ref(true)
const error = ref('')
const search = ref('')

// форма
const showForm = ref(false)
const editingId = ref<number | null>(null)
const saving = ref(false)
const formError = ref('')
const form = reactive({
  title: '',
  serialNumber: '',
  categoryId: 0,
  departmentId: 0,
})

const filtered = computed(() => {
  const q = search.value.trim().toLowerCase()
  if (!q) return units.value
  return units.value.filter(
    (u) =>
      u.title.toLowerCase().includes(q) ||
      u.serialNumber.toLowerCase().includes(q) ||
      u.categoryTitle.toLowerCase().includes(q) ||
      u.departmentTitle.toLowerCase().includes(q),
  )
})

onMounted(async () => {
  try {
    const [u, d, c] = await Promise.all([
      request<Unit[]>('/api/units'),
      request<Department[]>('/api/departments'),
      request<Category[]>('/api/categories'),
    ])
    units.value = u
    departments.value = d
    categories.value = c
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'Не удалось загрузить данные'
  } finally {
    loading.value = false
  }
})

function openCreate() {
  editingId.value = null
  form.title = ''
  form.serialNumber = ''
  form.categoryId = categories.value[0]?.id ?? 0
  form.departmentId = departments.value[0]?.id ?? 0
  formError.value = ''
  showForm.value = true
}

function openEdit(u: Unit) {
  editingId.value = u.id
  form.title = u.title
  form.serialNumber = u.serialNumber
  form.categoryId = u.categoryId
  form.departmentId = u.departmentId
  formError.value = ''
  showForm.value = true
}

function closeForm() {
  if (saving.value) return
  showForm.value = false
}

async function save() {
  if (!form.title.trim() || !form.serialNumber.trim()) {
    formError.value = 'Заполните название и серийный номер'
    return
  }
  if (!form.categoryId || !form.departmentId) {
    formError.value = 'Выберите категорию и отдел'
    return
  }

  saving.value = true
  formError.value = ''
  try {
    if (editingId.value === null) {
      const created = await sendJson<Unit>('/api/unit', 'POST', {
        title: form.title.trim(),
        serialNumber: form.serialNumber.trim(),
        categoryId: form.categoryId,
        departmentId: form.departmentId,
      })
      units.value.push(created)
    } else {
      const updated = await sendJson<Unit>('/api/unit', 'PUT', {
        id: editingId.value,
        title: form.title.trim(),
        serialNumber: form.serialNumber.trim(),
        categoryId: form.categoryId,
        departmentId: form.departmentId,
      })
      const index = units.value.findIndex((x) => x.id === editingId.value)
      if (index !== -1) units.value[index] = updated
    }
    showForm.value = false
  } catch (e) {
    formError.value = e instanceof Error ? e.message : 'Не удалось сохранить'
  } finally {
    saving.value = false
  }
}

async function remove(u: Unit) {
  if (!confirm(`Удалить «${u.title}» (${u.serialNumber})?`)) return
  try {
    await request<unknown>(`/api/unit/${u.id}`, { method: 'DELETE' })
    units.value = units.value.filter((x) => x.id !== u.id)
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'Не удалось удалить'
  }
}
</script>

<template>
  <section class="page">
    <div class="toolbar">
      <button class="btn btn-primary" :disabled="loading || !!error" @click="openCreate">
        <Plus :size="16" /> Добавить
      </button>
      <label class="search">
        <Search :size="16" />
        <input v-model="search" class="field" placeholder="Поиск..." />
      </label>
    </div>

    <div class="table-area">
      <p v-if="loading" class="info">Загрузка...</p>
      <p v-else-if="error" class="info error">{{ error }}</p>
      <p v-else-if="units.length === 0" class="info">ТМЦ пока нет. Нажмите «Добавить».</p>

      <table v-else>
        <thead>
          <tr>
            <th class="num">№</th>
            <th>Название</th>
            <th>Серийный номер</th>
            <th>Категория</th>
            <th>Отдел</th>
            <th class="actions-col"></th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(u, index) in filtered" :key="u.id">
            <td class="num">{{ index + 1 }}</td>
            <td>{{ u.title }}</td>
            <td>{{ u.serialNumber }}</td>
            <td>{{ u.categoryTitle }}</td>
            <td>{{ u.departmentTitle }}</td>
            <td class="actions">
              <button class="icon-btn" title="Редактировать" @click="openEdit(u)">
                <Pencil :size="16" />
              </button>
              <button class="icon-btn danger" title="Удалить" @click="remove(u)">
                <Trash2 :size="16" />
              </button>
            </td>
          </tr>
          <tr v-if="filtered.length === 0">
            <td colspan="6" class="empty">Ничего не найдено</td>
          </tr>
        </tbody>
      </table>
    </div>

    <footer class="bottom-bar">
      <span class="hint">Всего: {{ units.length }}</span>
    </footer>

    <!-- форма -->
    <div v-if="showForm" class="overlay" @click.self="closeForm">
      <form class="modal" @submit.prevent="save">
        <h2>{{ editingId === null ? 'Новое ТМЦ' : 'Редактирование ТМЦ' }}</h2>

        <label>
          Название
          <input v-model="form.title" class="field" :disabled="saving" autofocus />
        </label>

        <label>
          Серийный номер
          <input v-model="form.serialNumber" class="field" :disabled="saving" />
        </label>

        <label>
          Категория
          <select v-model.number="form.categoryId" class="field" :disabled="saving">
            <option v-for="c in categories" :key="c.id" :value="c.id">{{ c.title }}</option>
          </select>
        </label>

        <label>
          Отдел
          <select v-model.number="form.departmentId" class="field" :disabled="saving">
            <option v-for="d in departments" :key="d.id" :value="d.id">{{ d.title }}</option>
          </select>
        </label>

        <p v-if="formError" class="error">{{ formError }}</p>

        <div class="modal-actions">
          <button type="button" class="btn" :disabled="saving" @click="closeForm">Отмена</button>
          <button type="submit" class="btn btn-primary" :disabled="saving">
            {{ saving ? 'Сохранение...' : 'Сохранить' }}
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
  width: 44px;
  text-align: right;
  background: #fff;
  color: #555;
}
td.empty {
  background: #fff;
  text-align: center;
  color: #777;
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
  width: 420px;
  max-width: calc(100vw - 32px);
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
  margin: 0 0 4px;
  font-size: 16px;
}
.modal label {
  display: flex;
  flex-direction: column;
  gap: 4px;
  font-size: 13px;
  color: #444;
}
.modal .field {
  padding: 6px 8px;
}
.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 8px;
  margin-top: 4px;
}
</style>
