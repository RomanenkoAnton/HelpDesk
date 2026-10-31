import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '@/views/HomeView.vue'
import DepartmentsView from '@/views/DepartmentsView.vue'
import UnitsView from '@/views/UnitsView.vue'
import TicketsView from '@/views/TicketsView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    { path: '/', name: 'home', component: HomeView },
    { path: '/departments', name: 'departments', component: DepartmentsView },
    { path: '/units', name: 'units', component: UnitsView },
    { path: '/tickets', name: 'tickets', component: TicketsView },
  ],
})

export default router
