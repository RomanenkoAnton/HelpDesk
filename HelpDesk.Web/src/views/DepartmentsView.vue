<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { Pencil, Check, X, ArrowLeft, Plus, Trash2 } from 'lucide-vue-next'
import { request, sendJson } from '@/api/request'
import type { Department } from '@/types/api'

const router = useRouter()

const departments = ref<Department[]>([])
const loading = ref(true)
const error = ref('')

// редактирование в строке
const editingId = ref<number | null>(null)
const editTitle = ref('')
const saving = ref(false)
const saveError = ref('')

// добавление
const showForm = ref(false)
const newTitle = ref('')
const creating = ref(false)
const formError = ref('')

onMounted(async () => {
  try {
    departments.value = await request<Department[]>('/api/departments')
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'Не удалось загрузить данные'
  } finally {
    loading.value = false
  }
})

function startEdit(d: Department) {
  editingId.value = d.id
  editTitle.value = d.title
  saveError.value = ''
}

function cancelEdit() {
  editingId.value = null
  saveError.value = ''
}

async function saveEdit(d: Department) {
  saving.value = true
  saveError.value = ''
  try {
    const updated = await sendJson<Department>('/api/department', 'PUT', {
      id: d.id,
      title: editTitle.value,
    })
    const index = departments.value.findIndex((x) => x.id === d.id)
    if (index !== -1) {
      departments.value[index] = updated
    }
    editingId.value = null
  } catch (e) {
    saveError.value = e instanceof Error ? e.message : 'Не удалось сохранить'
  } finally {
    saving.value = false
  }
}

function openCreate() {
  newTitle.value = ''
  formError.value = ''
  showForm.value = true
}

function closeForm() {
  if (creating.value) return
  showForm.value = false
}

async function create() {
  if (!newTitle.value.trim()) {
    formError.value = 'Введите название'
    return
  }
  creating.value = true
  formError.value = ''
  try {
    const created = await sendJson<Department>('/api/department', 'POST', {
      title: newTitle.value.trim(),
    })
    departments.value.push(created)
    showForm.value = false
  } catch (e) {
    formError.value = e instanceof Error ? e.message : 'Не удалось добавить'
  } finally {
    creating.value = false
  }
}

async function remove(d: Department) {
  if (!confirm(`Удалить подразделение «${d.title}»?`)) return
  saveError.value = ''
  try {
    await request<unknown>(`/api/department/${d.id}`, { method: 'DELETE' })
    departments.value = departments.value.filter((x) => x.id !== d.id)
  } catch (e) {
    saveError.value = e instanceof Error ? e.message : 'Не удалось удалить'
  }
}
</script>

<template>
  <section class="page">
    <div class="toolbar">
      <button class="btn btn-primary" :disabled="loading || !!error" @click="openCreate">
        <Plus :size="16" /> Добавить
      </button>
    </div>

    <div class="table-area">
      <p v-if="loading" class="info">Загрузка...</p>
      <p v-else-if="error" class="info error">{{ error }}</p>
      <p v-else-if="departments.length === 0" class="info">
        Подразделений пока нет. Нажмите «Добавить».
      </p>

      <table v-else>
        <thead>
          <tr>
            <th class="num">№</th>
            <th>Название</th>
            <th class="actions-col"></th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="(d, index) in departments"
            :key="d.id"
            :class="{ editing: editingId === d.id }"
          >
            <td class="num">{{ index + 1 }}</td>
            <td>
              <input
                v-if="editingId === d.id"
                v-model="editTitle"
                class="field"
                :disabled="saving"
                @keyup.enter="saveEdit(d)"
                @keyup.esc="cancelEdit"
              />
              <span v-else>{{ d.title }}</span>
            </td>
            <td class="actions">
              <template v-if="editingId === d.id">
                <button class="icon-btn ok" title="Сохранить" :disabled="saving" @click="saveEdit(d)">
                  <Check :size="16" />
                </button>
                <button class="icon-btn" title="Отмена" :disabled="saving" @click="cancelEdit">
                  <X :size="16" />
                </button>
              </template>
              <template v-else>
                <button class="icon-btn" title="Редактировать" @click="startEdit(d)">
                  <Pencil :size="16" />
                </button>
                <button class="icon-btn danger" title="Удалить" @click="remove(d)">
                  <Trash2 :size="16" />
                </button>
              </template>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <footer class="bottom-bar">
      <span v-if="saveError" class="error">{{ saveError }}</span>
      <span v-else class="hint">Enter - сохранить, Esc - отмена</span>
      <button class="btn" @click="router.back()">
        <ArrowLeft :size="16" /> Назад
      </button>
    </footer>

    <!-- форма добавления -->
    <div v-if="showForm" class="overlay" @click.self="closeForm">
      <form class="modal" @submit.prevent="create">
        <h2>Новое подразделение</h2>

        <label>
          Название
          <input v-model="newTitle" class="field" :disabled="creating" autofocus />
        </label>

        <p v-if="formError" class="error">{{ formError }}</p>

        <div class="modal-actions">
          <button type="button" class="btn" :disabled="creating" @click="closeForm">Отмена</button>
          <button type="submit" class="btn btn-primary" :disabled="creating">
            {{ creating ? 'Сохранение...' : 'Добавить' }}
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
  max-width: 640px;
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
td.num,
th.num {
  width: 44px;
  text-align: right;
  background: #fff;
  color: #555;
}
tbody tr:hover td:not(.num) {
  background: #e8fbcf;
}
tbody tr.editing td:not(.num) {
  background: var(--yellow-soft);
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
.icon-btn.ok:hover {
  color: #2e7d32;
}
.icon-btn.danger:hover {
  color: #c62828;
}

td .field {
  padding: 1px 6px;
  font-size: 13px;
  height: 22px;
}

.bottom-bar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
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
  width: 380px;
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
  height: auto;
  font-size: 14px;
}
.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 8px;
  margin-top: 4px;
}
</style>
