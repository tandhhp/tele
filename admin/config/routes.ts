export default [
  {
    path: '/',
    redirect: '/home',
  },
  {
    path: '/accounts',
    layout: false,
    routes: [
      {
        name: 'login',
        path: '/accounts/login',
        component: './accounts/login'
      }
    ],
  },
  {
    icon: 'HomeOutlined',
    name: 'Trang chủ',
    path: '/home',
    component: './Home',
  },
  {
    name: 'Lịch làm việc',
    icon: 'CalendarOutlined',
    path: '/calendar',
    component: './calendar',
    hideInMenu: true
  },
  {
    name: 'component',
    path: '/works',
    hideInMenu: true,
    routes: [
      {
        name: 'block',
        path: '/works/:id',
        component: './works',
        hideInMenu: true,
      },
      {
        name: 'articleLister',
        path: '/works/article-lister/:id',
        component: './works/article-lister',
        hideInMenu: true,
      },
      {
        name: 'articlePicker',
        path: '/works/article-picker/:id',
        component: './works/article-picker',
        hideInMenu: true,
      },
      {
        name: 'articleSpotlight',
        path: '/works/article-spotlight/:id',
        component: './works/article-spotlight',
        hideInMenu: true,
      },
      {
        name: 'block',
        path: '/works/block/:id',
        component: './works/block',
        hideInMenu: true,
      },
      {
        name: 'editor',
        path: '/works/editor/:id',
        component: './works/editor',
        hideInMenu: true,
      },
      {
        name: 'facebookAlbum',
        path: '/works/facebook-album/:id',
        component: './works/facebook/album',
        hideInMenu: true,
      },
      {
        name: 'contactForm',
        path: '/works/contact-form/:id',
        component: './works/contact-form',
      },
      {
        name: 'feed',
        path: '/works/feed/:id',
        component: './works/feed',
      },
      {
        name: 'row',
        path: '/works/row/:id',
        component: './works/row',
        hideInMenu: true,
      },
      {
        name: 'image',
        path: '/works/image/:id',
        component: './works/image',
        hideInMenu: true,
      },
      {
        name: 'navbar',
        path: '/works/navbar/:id',
        component: './works/navbar',
        hideInMenu: true,
      },
      {
        name: 'swiper',
        path: '/works/swiper/:id',
        component: './works/swiper',
        hideInMenu: true,
      },
      {
        name: 'blockEditor',
        path: '/works/blockeditor/:id',
        component: './works/block-editor',
        hideInMenu: true,
      },
      {
        name: 'card',
        path: '/works/card/:id',
        component: './works/card',
        hideInMenu: true,
      },
      {
        name: 'exchangeRate',
        path: '/works/exchange-rate/:id',
        component: './works/exchange-rate',
        hideInMenu: true,
      },
      {
        name: 'googleMap',
        path: '/works/googlemap/:id',
        component: './works/google-map',
        hideInMenu: true,
      },
      {
        name: 'jumbotron',
        path: '/works/jumbotron/:id',
        component: './works/jumbotron',
        hideInMenu: true,
      },
      {
        name: 'masonry',
        path: '/works/masonry/:id',
        component: './works/masonry',
        hideInMenu: true,
      },
      {
        name: 'lookbook',
        path: '/works/lookbook/:id',
        component: './works/lookbook',
        hideInMenu: true,
      },
      {
        name: 'tag',
        path: '/works/tag/:id',
        component: './works/tag',
        hideInMenu: true,
      },
      {
        name: 'link',
        path: '/works/link/:id',
        component: './works/link',
        hideInMenu: true,
      },
      {
        name: 'listGroup',
        path: '/works/list-group/:id',
        component: './works/list-group',
        hideInMenu: true,
      },
      {
        name: 'productLister',
        path: '/works/product-lister/:id',
        component: './works/product-lister',
        hideInMenu: true,
      },
      {
        name: 'productPicker',
        path: '/works/product-picker/:id',
        component: './works/product-picker',
        hideInMenu: true,
      },
      {
        name: 'productSpotlight',
        path: '/works/product-spotlight/:id',
        component: './works/product-spotlight',
        hideInMenu: true,
      },
      {
        name: 'shopeeProduct',
        path: '/works/shopee-product/:id',
        component: './works/shopee-product',
        hideInMenu: true,
      },
      {
        name: 'sponsor',
        path: '/works/sponsor/:id',
        component: './works/sponsor',
        hideInMenu: true,
      },
      {
        name: 'trend',
        path: '/works/trend/:id',
        component: './works/trend',
        hideInMenu: true,
      },
      {
        name: 'videoPlayer',
        path: '/works/video-player/:id',
        component: './works/video-player',
        hideInMenu: true,
      },
      {
        name: 'videoPlaylist',
        path: '/works/video-playlist/:id',
        component: './works/video-playlist',
        hideInMenu: true,
      },
      {
        name: 'wordPressLister',
        path: '/works/wordpress-lister/:id',
        component: './works/wordpress-lister',
        hideInMenu: true,
      }
    ],
  },
  {
    icon: 'IdcardOutlined',
    name: 'Liên hệ',
    path: '/contact',
    routes: [
      {
        name: 'Danh bạ',
        path: '/contact/index',
        component: './contact'
      },
      {
        name: 'Chi tiết liên hệ',
        path: '/contact/center/:id',
        component: './contact/center',
        hideInMenu: true
      },
      {
        name: 'Blacklist',
        path: '/contact/blacklist',
        component: './contact/blacklist'
      }
    ]
  },
  {
    icon: 'TeamOutlined',
    name: 'Người dùng',
    path: '/user',
    routes: [
      {
        name: 'Hồ sơ',
        path: '/user/profile',
        component: './user/profile',
        hideInMenu: true,
      },
      {
        name: 'Bảo mật',
        path: '/user/setting',
        component: './user/setting',
        hideInMenu: true,
      },
      {
        name: 'Nhân viên',
        path: '/user/roles',
        component: './user/roles'
      },
      {
        name: 'Chức vụ',
        path: '/user/roles/:id',
        component: './user/roles/center',
        hideInMenu: true
      },
      {
        name: 'Thông báo',
        path: '/user/notification',
        component: './notification',
        hideInMenu: true
      },
      {
        name: 'Phòng - Ban',
        path: '/user/department',
        component: './department'
      },
      {
        name: 'Team',
        path: '/user/department/team/:id',
        component: './department/team',
        hideInMenu: true
      },
      {
        name: 'Thành viên',
        path: '/user/department/team/user/:id',
        component: './user/team/user',
        hideInMenu: true
      },
      {
        name: 'Tài khoản',
        path: '/user/account',
        component: './user'
      }
    ],
  },
  {
    icon: 'AccountBookOutlined',
    name: 'Kế toán',
    path: '/accountant',
    access: 'canAccountant',
    routes: [
      {
        name: 'Công nợ sự kiện',
        path: '/accountant/event',
        component: './accountant/event'
      },
      {
        name: 'Báo cáo doanh số',
        path: '/accountant/report',
        component: './debt'
      },
      {
        name: 'Báo cáo điểm',
        path: '/accountant/loyalty-report',
        component: './accountant/loyalty-report'
      }
    ]
  },
  {
    icon: 'AccountBookOutlined',
    name: 'Giám đốc kinh doanh',
    path: '/dos',
    access: 'canDos',
    routes: [
      {
        name: 'Công nợ sự kiện',
        path: '/dos/event',
        component: './accountant/event'
      }
    ]
  },
  {
    icon: 'TeamOutlined',
    name: 'Khách Plasma',
    path: '/plasma',
    access: 'canPlasma',
    component: './plasma'
  },
  {
    icon: 'CalendarOutlined',
    name: 'CheckIn Plasma',
    path: '/plasmaCheckIn',
    access: 'plasma',
    component: './plasmaCheckIn'
  },
  {
    icon: 'SettingOutlined',
    name: 'Cài đặt',
    path: '/settings',
    access: 'canAdmin',
    routes: [
      {
        path: '/settings',
        redirect: '/settings/general',
      },
      {
        path: '/settings/general',
        name: 'Cài đặt chung',
        component: './settings'
      },
      {
        name: 'component',
        path: '/settings/component',
        component: './settings/components',
        hideInMenu: true,
      },
      {
        name: 'componentCenter',
        path: '/settings/component/center/:id',
        component: './settings/components/center',
        hideInMenu: true,
      },
      {
        name: 'Tỉnh/Thành phố',
        path: '/settings/province',
        component: './settings/province'
      },
      {
        name: 'Xã/Phường',
        path: '/settings/province/district/:id',
        component: './settings/province/district',
        hideInMenu: true,
      },
      {
        name: 'Nghề nghiệp',
        path: '/settings/job-kind',
        component: './settings/job-kind'
      },
      {
        name: 'Chi nhánh',
        path: '/settings/branch',
        component: './settings/branch'
      }
    ],
  },
  {
    icon: 'CalendarOutlined',
    name: 'Sự kiện',
    path: '/event',
    routes: [
      {
        name: 'Sự kiện',
        path: '/event/index',
        component: './event'
      },
      {
        name: 'Phòng',
        path: '/event/room',
        component: './event/room'
      },
      {
        name: 'Bàn',
        path: '/event/room/table/:id',
        component: './event/room/table',
        hideInMenu: true
      },
      {
        name: 'Chiến dịch',
        path: '/event/campaign',
        component: './event/campaign'
      }
    ]
  },
  {
    name: 'Người tham dự',
    path: '/event/user/:id',
    component: './event/user',
    hideInMenu: true
  },
  {
    name: 'Chat',
    component: './chat',
    path: '/chat',
    hideInMenu: true
  },
  {
    name: 'Lịch sử',
    path: '/history',
    component: './history',
    icon: 'HistoryOutlined'
  },
  {
    path: '*',
    layout: false,
    component: './404',
  }
]