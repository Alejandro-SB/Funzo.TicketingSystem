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
        children: [
          {
            path: ':id',
            component: () => import('pages/TicketDetailPage.vue'),
          },
        ],
      },
      {
        path: '/users',
        component: () => import('pages/UsersPage.vue'),
        children: [
          {
            path: ':id',
            component: () => import('pages/UserDetailPage.vue'),
          },
          {
            path: ':id/comments',
            component: () => import('pages/UserCommentsPage.vue'),
          },
        ],
      },
    ],
  },

  // Always leave this as last one,
  // but you can also remove it
  {
    path: '/:catchAll(.*)*',
    component: () => import('pages/ErrorNotFound.vue'),
  },
];

export default routes;
