import { apiCardHolderBirthDays } from '@/services/catalog';
import { PageContainer, ProTable } from '@ant-design/pro-components';
import { Col, Row, Select, Space } from 'antd';
import NewCardHolder from './components/new-cardholder';
import AmountReport from './components/amount';
import SaleChart from './components/sale-chart';
import TopSales from './components/top-month';
import SmChart from './components/sm-chart';
import { useAccess, useModel } from '@umijs/max';
import LineBranch from './components/line-branch';
import dayjs from 'dayjs';
import { useState } from 'react';
import TrainerComponent from './components/trainer';
import TeleReport from './components/tele';
import EventDashboard from './components/event';
import SaleRevenue from './components/sale-revenue';
import { BRANCH_OPTIONS } from '@/utils/constants';

const HomePage: React.FC = () => {

  const access = useAccess();
  const [branch, setBranch] = useState<number>(-1);
  const { initialState } = useModel('@@initialState');

  const canViewBirthday = () => {
    if (access.cx || access.cxm || access.dos || access.canAdmin) return true;
    return false;
  }

  const canViewNewCardHolder = () => {
    if (access.canAdmin || access.dos || access.cx || access.cxm) return true;
    return false;
  }

  return (
    <PageContainer extra={(
      <Space>
        <Select
          disabled={!access.canAdmin}
          options={BRANCH_OPTIONS}
          defaultValue={access.canAdmin ? null : initialState?.currentUser?.branch}
          className="w-32"
          onChange={(v) => setBranch(v)} />
      </Space>
    )}>
      {access.event && <EventDashboard />}
      {
        access.telesaleManager && <TeleReport />
      }
      {
        access.trainer && <TrainerComponent />
      }
      {
        access.sales && <SaleRevenue />
      }
      <AmountReport />
      <Row gutter={16}>
        <Col xs={24} md={18}>
          <Row gutter={16}>
            <Col md={12} xs={24} hidden={!access.canViewChart}>
              <SaleChart />
            </Col>
            <Col md={12} xs={24} hidden={!access.canViewChart}>
              <SmChart />
            </Col>
          </Row>
        </Col>
        <Col xs={24} md={6} hidden={!access.canViewChart}>
          <TopSales />
        </Col>
        <LineBranch branch={branch} />
        <Col md={12} xs={24} className='mb-4' hidden={!canViewBirthday()}>
          <div className='bg-white h-full'>
            <ProTable
              request={apiCardHolderBirthDays}
              columns={[
                {
                  title: '#',
                  valueType: 'indexBorder',
                  width: 40
                },
                {
                  title: 'Họ & Tên',
                  dataIndex: 'name'
                },
                {
                  title: 'Ngày sinh',
                  dataIndex: 'dateOfBirth',
                  valueType: 'date',
                  render: (_, entity) => entity.dateOfBirth ? dayjs(entity.dateOfBirth).format('DD-MM-YYYY') : '-'
                },
                {
                  title: 'Giới tính',
                  dataIndex: 'gender',
                  render: (dom, entity) => entity.gender === true ? "Nam" : "Nữ",
                  width: 80
                }
              ]}
              rowKey="id"
              pagination={{
                defaultPageSize: 10
              }}
              search={false} headerTitle="Sinh nhật chủ thẻ" />
          </div>
        </Col>
        <Col md={12} xs={24} className='mb-4' hidden={!canViewNewCardHolder()}>
          <div className='bg-white h-full'>
            <NewCardHolder />
          </div>
        </Col>
      </Row>
    </PageContainer>
  );
};

export default HomePage;
