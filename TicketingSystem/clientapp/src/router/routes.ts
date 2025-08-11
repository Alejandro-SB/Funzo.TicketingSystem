import type { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    component: () => import('layouts/MainLayout.vue'),
    children: [
      { path: '', component: () => import('pages/IndexPage.vue') },
      {
        path: '/tickets',
        component: () => import('pages/TicketsPage.vue'),
      },
      {
        path: '/tickets/:id',
        component: () => import('pages/TicketDetailPage.vue'),
      },
      {
        path: '/users',
        component: () => import('pages/UsersPage.vue'),
      },
      {
        path: '/users/:id',
        component: () => import('pages/UserDetailPage.vue'),
      },
    ],
  },

  // Always leave this as last one,
  // but you can also remove it
  {
    path: '/:catchAll(.*)*',
    name: 'not-found',
    component: () => import('pages/ErrorNotFound.vue'),
  },
];

export default routes;
