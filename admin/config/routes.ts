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
    name: 'Nội dung',
    path: '/catalog',
    icon: 'SlackOutlined',
    access: 'canCX',
    routes: [
      {
        name: 'Chăm sóc sức khỏe',
        path: '/catalog/product',
        component: './catalog/products'
      },
      {
        name: 'Cơ sở khám',
        path: '/catalog/hospital',
        component: './catalog/hospital'
      },
      {
        name: 'Gói khám',
        path: '/catalog/hospital/package/:id',
        component: './catalog/hospital/package',
        hideInMenu: true
      },
      {
        name: 'Dưỡng sinh độc bản',
        path: '/catalog/tour',
        component: './tour'
      },
      {
        name: 'Danh sách khách sạn',
        path: '/catalog/room',
        component: './tour/room'
      },
      {
        name: 'Tin tức',
        path: '/catalog/article',
        component: './catalog/article',
        hideInMenu: true
      },
      {
        name: 'Trang',
        path: '/catalog/page',
        component: './catalog/page'
      },
      {
        name: 'Địa điểm',
        path: '/catalog/city',
        component: './catalog/city'
      },
      {
        name: 'Bình luận',
        path: '/catalog/comments',
        component: './comments'
      },
      {
        name: 'Thành tựu',
        icon: 'TrophyOutlined',
        path: '/catalog/achievement',
        component: './achievement',
        access: 'canCXM'
      }
    ]
  },
  {
    name: 'catalog',
    path: '/catalog/:id',
    component: './catalog',
    hideInMenu: true
  },
  {
    icon: 'ShoppingCartOutlined',
    name: 'Đơn đăng ký',
    path: '/form',
    component: './tour/forms',
    access: 'canForm'
  },
  {
    icon: 'TeamOutlined',
    name: 'Người dùng',
    path: '/users',
    routes: [
      {
        name: 'Chủ thẻ',
        path: '/users/member',
        component: './users/member',
        access: 'canCardHolder'
      },
      {
        name: 'Hồ sơ',
        path: '/users/member/:id',
        component: './users/profile',
        hideInMenu: true,
      },
      {
        name: 'Bảo mật',
        path: '/users/center/:id',
        component: './users/center',
        hideInMenu: true,
      },
      {
        name: 'Hoạt động',
        path: '/users/contact/activity/:id',
        component: './users/contact/activity',
        hideInMenu: true
      },
      {
        name: 'Nhân viên',
        path: '/users/roles',
        component: './users/roles',
        access: 'canHR',
      },
      {
        name: 'Chức vụ',
        path: '/users/roles/:id',
        component: './users/roles/center',
        hideInMenu: true
      },
      {
        name: 'Liên hệ',
        path: '/users/contact',
        component: './users/contact',
        access: 'canCX',
      },
      {
        name: 'Team',
        path: '/users/team',
        component: './users/team'
      },
      {
        name: 'Thay đổi nhân sự',
        path: '/users/changes',
        component: './users/changes',
        access: 'canUserChange'
      },
      {
        name: 'Thông báo',
        path: '/users/notification',
        component: './notification',
        hideInMenu: true
      },
      {
        name: 'Blacklist',
        path: '/users/blacklist',
        component: './users/contact/blacklist'
      },
      {
        name: 'Phòng - Ban',
        path: '/users/department',
        component: './department'
      },
      {
        name: 'Nhóm',
        path: '/users/department/team/:id',
        component: './department/team',
        hideInMenu: true
      },
      {
        name: 'Thành viên trong nhóm',
        path: '/users/department/team/user/:id',
        component: './users/team/user',
        hideInMenu: true
      },
      {
        name: 'Tài khoản',
        path: '/users/account',
        component: './users'
      }
    ],
  },
  {
    icon: 'CalculatorOutlined',
    name: 'Duyệt Top-Up / Công nợ',
    path: '/users/top-up',
    component: './top-up',
    access: 'canDosAccountant',
  },
  {
    icon: 'UserSwitchOutlined',
    name: 'Chuyển đổi khách hàng',
    path: '/card-holder-queue',
    component: './users/queue',
    access: 'canCardHolderQueue'
  },
  {
    icon: 'UsergroupAddOutlined',
    name: 'Khách hàng tiềm năng',
    path: '/lead',
    component: './users/lead',
    access: 'canLead',
  },
  {
    icon: 'AccountBookOutlined',
    name: 'Kế toán',
    path: '/accountant',
    access: 'canAccountant',
    routes: [
      {
        name: 'Duyệt điểm',
        path: '/accountant/loyalty',
        component: './users/loyalty'
      },
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
        name: 'Thẻ',
        path: '/settings/card',
        component: './users/card',
        access: 'canAdmin',
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
        name: 'google',
        path: '/settings/google/:id',
        component: './settings/google',
        hideInMenu: true,
      },
      {
        name: 'footer',
        path: '/settings/footer/:id',
        component: './settings/footer',
        hideInMenu: true,
      },
      {
        name: 'header',
        path: '/settings/header/:id',
        component: './settings/header',
        hideInMenu: true,
      },
      {
        name: 'style',
        path: '/settings/css',
        component: './settings/css',
        hideInMenu: true,
      },
      {
        name: 'telegram',
        path: '/settings/telegram/:id',
        component: './settings/telegram',
        hideInMenu: true,
      },
      {
        name: 'sendGrid',
        path: '/settings/sendgrid/:id',
        component: './settings/sendgrid',
        hideInMenu: true,
      },
      {
        name: 'facebook',
        path: '/settings/facebook/:id',
        component: './settings/facebook',
        hideInMenu: true,
      },
      {
        name: 'social',
        path: '/settings/social/:id',
        component: './settings/social',
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
      }
    ],
  },
  {
    icon: 'CalendarOutlined',
    name: 'Sự kiện',
    path: '/event',
    access: 'canEvent',
    routes: [
      {
        name: 'Sự kiện 09:00',
        path: '/event/am',
        component: './event/user'
      },
      {
        name: 'Sự kiện 14:30',
        path: '/event/pm',
        component: './event/user'
      },
      {
        name: 'Check-In',
        path: '/event/checkin/:id',
        component: './event/user',
        hideInMenu: true
      },
      {
        name: 'Feedback',
        path: '/event/feedback',
        component: './event/feedback',
        access: 'canAdmin'
      },
      {
        name: 'Key-In của tôi',
        path: '/event/my-keyin',
        component: './event/my-keyin'
      },
      {
        name: 'Công nợ',
        path: '/event/debt',
        component: './event/debt',
        access: 'event'
      }
    ]
  },
  {
    name: 'Người tham dự',
    path: '/event/user/:id',
    component: './event/user',
    access: 'canCX',
    hideInMenu: true
  },
  {
    name: 'Chat',
    component: './chat',
    path: '/chat',
    hideInMenu: true
  },
  {
    path: '*',
    layout: false,
    component: './404',
  }
]