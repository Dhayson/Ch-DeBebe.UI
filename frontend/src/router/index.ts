import { createRouter, createWebHistory } from 'vue-router';
import LoginView from '../views/LoginView.vue';

const router = createRouter({
  // Define o modo de histórico (HTML5), que remove o '#' da URL
  history: createWebHistory(import.meta.env.BASE_URL),
  
  routes: [
    {
      path: '/login',
      name: 'login',
      component: LoginView
    },
    {
      path: '/cadastro',
      name: 'cadastro',
      // Lazy loading: o componente só é baixado quando o usuário acessa a rota
      component: () => import('../views/CadastroView.vue')
    },
    {
      path: '/dashboard',
      name: 'dashboard',
      component: () => import('../views/DashboardView.vue'),
      // Metadado para indicar que esta rota precisa de login
      meta: { requiresAuth: true }
    },
    {
      path: '/gerenciar/:id',
      name: 'GerenciarCha',
      component: () => import('../views/GerenciarChaView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/convite/:codigo',
      name: 'Convite',
      component: () => import('../views/ConviteView.vue')
    },
    {
      path: '/cha_de_bebe/:id',
      name: 'VisualizarCha',
      component: () => import('../views/VisualizarChaView.vue')
    }
  ]
});

router.beforeEach((to, from, next) => {
  const isAuthenticated = !!localStorage.getItem('token');
  if (to.path === '/') {
    if (isAuthenticated) {
      next('/dashboard'); // Já está logado
    } else {
      next('/login');     // Precisa logar
    }
    return;
  }
  // Verifica se a rota de destino exige autenticação
  if (to.meta.requiresAuth && !isAuthenticated) {
    // Se não estiver logado e tentar entrar no dashboard, manda pro login
    next({ name: 'login' });
  } else {
    // Caso contrário, segue o fluxo normal
    next();
  }
});

export default router;