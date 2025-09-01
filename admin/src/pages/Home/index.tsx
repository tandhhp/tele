import { apiCardHolderBirthDays } from '@/services/catalog';
import { PageContainer, ProForm, ProFormSelect, ProTable } from '@ant-design/pro-components';
import { Col, Row } from 'antd';
import NewCardHolder from './components/new-cardholder';
import AmountReport from './components/amount';
import SaleChart from './components/sale-chart';
import TopSales from './components/top-month';
import SmChart from './components/sm-chart';
import { useAccess } from '@umijs/max';
import LineBranch from './components/line-branch';
import dayjs from 'dayjs';
import TrainerComponent from './components/trainer';
import TeleReport from './components/tele';
import EventDashboard from './components/event';
import SaleRevenue from './components/sale-revenue';
import { apiBranchOptions } from '@/services/settings/branch';
import ContactStatistics from './components/contact';

const HomePage: React.FC = () => {

  const access = useAccess();

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
      <ProForm submitter={false}>
        <ProFormSelect request={apiBranchOptions} className="w-32" name={`branchId`} formItemProps={{
          className: 'mb-0'
        }} initialValue={1} />
      </ProForm>
    )}>
      <div className='grid grid-cols-1 md:grid-cols-4 gap-4 mb-4'>
        <ContactStatistics />
        <ContactStatistics />
        <ContactStatistics />
        <ContactStatistics />
      </div>
      <Row gutter={16}>
        <Col xs={24} md={18}>
          <Row gutter={16}>
            <Col md={12} xs={24}>
              <SaleChart />
            </Col>
            <Col md={12} xs={24}>
              <SmChart />
            </Col>
          </Row>
        </Col>
        <Col xs={24} md={6}>
          <TopSales />
        </Col>
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
